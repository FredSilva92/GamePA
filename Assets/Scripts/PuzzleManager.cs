using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utils;

public class PuzzleManager : MonoBehaviour
{
    /* ATRIBUTOS */

    [SerializeField] private int[] _pieceOrder = { 6, 9, 8, 7, 2, 4, 5, 3, 1 };

    [SerializeField] private List<Transform> _wallPoints;
    [SerializeField] private List<PuzzlePiece> _pieces;

    private PuzzlePiece _firstPiece = null;
    private PuzzlePiece _secondPiece = null;

    private float _firstPieceToFrontDistance = 0.81f;
    private float _secondPieceToFrontDistance = 0.45f;

    private bool _isSolving = false;

    [SerializeField] private float _moveDuration = 1f;

    [SerializeField] private GameObject _door;
    private DoorAnimationManager _doorScript;

    [SerializeField] private GameObject _pyramidEntranceCollider;

    [SerializeField] private GameObject _walkToPuzzlePoint;
    [SerializeField] private GameObject _lookToPuzzlePoint;

    private bool _walkStarted = false;
    private bool _lookStarted = false;

    private bool _isMovingFirstPiece = false;
    private bool _isMovingSecondPiece = false;

    [SerializeField] private AudioSource _pieceDragAudio;


    /* PROPRIEDADES */

    public List<PuzzlePiece> Pieces
    {
        get { return _pieces; }
        set { _pieces = value; }
    }

    public bool IsSolving
    {
        get { return _isSolving; }
        set { _isSolving = value; }
    }


    /* MÉTODOS */

    /*
     * Embaralha a lista das peças do puzzle, de acordo com a ordem dada. 
     * E atualiza as posições das peças na parede.
    */
    private void Start()
    {
        _doorScript = _door.GetComponent<DoorAnimationManager>();
        _pyramidEntranceCollider.SetActive(false);

        ShufflePuzzle();

        // executar o código se for a IA a jogar
        if (SceneManager.GetActiveScene().name == "SolvePuzzleAI")
        {
            AgentsWorking = false;
        }
    }

    private void FixedUpdate()
    {
        if (_walkStarted)
        {
            WalkToPuzzle();
        }
        if (_lookStarted)
        {
            LookToPuzzle();
        }

        // executar o código se for a IA a jogar
        if (SceneManager.GetActiveScene().name == "SolvePuzzleAI")
        {
            if (Training)
            {
                Timeout += Time.fixedDeltaTime;

                //if (Timeout > 2.0f)
                //{
                //    Timeout = 0.0f;
                //    ResetGame();
                //}
            }

            //if (PlayerO.AgentStatusAI == AgentStatusAI.Ready && PlayerX.AgentStatusAI == AgentStatusAI.Ready)
            //{
            //    PlayerO.AgentStatusAI = AgentStatusAI.Working;
            //    PlayerX.AgentStatusAI = AgentStatusAI.Working;

            //    InitialiseGame();
            //    AgentsWorking = true;
            //}

            if (GameStatusAI == GameStatusAI.ReadyToMove && AgentsWorking)
            {
                GameStatusAI = GameStatusAI.PerformingMove;
                Timeout = 0;

                if (CurrentPlayer == PlayerTypeAI.player1)
                {
                    Debug.Log("Turno do player1");
                    RequestDecision(Player1);
                }
                else
                {
                    Debug.Log("Turno do player2");
                    RequestDecision(Player2);
                }
            }
            else if (GameStatusAI == GameStatusAI.ObserveMove)
            {
                GameStatusAI = GameStatusAI.ObservingMove;
                Timeout = 0;

                if (CurrentPlayer == PlayerTypeAI.player1)
                {
                    RequestDecision(Player1);
                }
                else
                {
                    RequestDecision(Player2);
                }
            }
            else if (GameStatusAI == GameStatusAI.ChangePlayer)
            {
                GameStatusAI = GameStatusAI.ChangingPlayer;
                Timeout = 0;
                ChangePlayer();
            }
            else if (GameStatusAI == GameStatusAI.GiveRewards)
            {
                GameStatusAI = GameStatusAI.GiveRewards;
                Timeout = 0;
                AgentsWorking = false;

                Player2.AgentStatusAI = AgentStatusAI.Resetting;
                Player1.AgentStatusAI = AgentStatusAI.Resetting;

                GiveRewards();
            }
            else if (GameStatusAI == GameStatusAI.FinalObservation)
            {
                GameStatusAI = GameStatusAI.MakingFinalObservation;
                Timeout = 0;
                RequestDecision(Player1);
                RequestDecision(Player2);
            }
            else if (Player1.AgentStatusAI == AgentStatusAI.MadeFinalObservation && Player2.AgentStatusAI == AgentStatusAI.MadeFinalObservation)
            {
                Timeout = 0;
                Player1.AgentStatusAI = AgentStatusAI.EndingGame;
                Player2.AgentStatusAI = AgentStatusAI.EndingGame;
                GameStatusAI = GameStatusAI.EndingGame;

                EndGame();
            }
        }
    }

    private void ShufflePuzzle()
    {
        _pieces = _pieces.OrderBy(piece => Array.IndexOf(_pieceOrder, piece.position)).ToList();

        for (int i = 0; i < _pieces.Count; i++)
        {
            Vector3 newPosition = _wallPoints[i].localPosition;
            UpdatePosition(i, newPosition);
        }
    }

    public void DoPlay()
    {
        // se uma tecla for pressionada e não existe nenhuma peça a mover-se
        if (Input.anyKeyDown && !_isMovingFirstPiece && !_isMovingSecondPiece)
        {
            // obtém o número da tecla
            int inputtedNumber = GetNumericKeyValue();

            if (CheckValidPlay(inputtedNumber))
            {
                // se AINDA NÃO escolheu a primeira peça, escolhe essa
                // se JÁ escolheu a primeira, agora escolhe a segunda
                if (_firstPiece == null)
                {
                    _firstPiece = ChoosePiece(inputtedNumber);
                    StartCoroutine(MoveToFront(_firstPiece, _firstPieceToFrontDistance));
                }
                else
                {
                    _secondPiece = ChoosePiece(inputtedNumber);
                    StartCoroutine(MoveSecondPieceThenSwap());
                }
            }
        }
    }

    /*
     * Recebe a tecla pressionada e converte o valor.
     * 0 a 9 - se a tecla for um número.
     * -1 - se não for.
    */
    private int GetNumericKeyValue()
    {
        string key = Input.inputString;
        int result;

        if (int.TryParse(key, out result))
        {
            return result;
        }
        else
        {
            return -1;
        }
    }

    private bool CheckValidPlay(int inputtedNumber)
    {
        if (inputtedNumber >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsSamePiece()
    {
        if (_firstPiece == _secondPiece)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private PuzzlePiece ChoosePiece(int inputtedNumber)
    {
        PuzzlePiece piece = _pieces[inputtedNumber - 1];

        if (piece != null)
        {
            return piece;
        }
        else
        {
            return null;
        }
    }

    private IEnumerator MoveSecondPieceThenSwap()
    {
        _isMovingSecondPiece = true;

        Vector3 endStartPosition = new Vector3(_secondPiece.piece.transform.position.x, _secondPiece.piece.transform.position.y, _secondPiece.piece.transform.position.z);

        if (IsSamePiece())
        {
            yield return StartCoroutine(MoveToBack(_firstPiece, _firstPieceToFrontDistance));
        }
        else
        {
            yield return StartCoroutine(MoveToFront(_secondPiece, _secondPieceToFrontDistance));
            yield return StartCoroutine(MoveSecondToFirstPiece());
            yield return StartCoroutine(MoveToBack(_secondPiece, _secondPieceToFrontDistance));
            yield return StartCoroutine(MoveFirstToSecondPiece(endStartPosition));

            int firstIndexOfList = _pieces.IndexOf(_firstPiece);
            int secondIndeOfList = _pieces.IndexOf(_secondPiece);
            SwapPieceInList(firstIndexOfList, secondIndeOfList);
        }

        ResetValues();

        _isMovingSecondPiece = false;
    }

    private void SwapPieceInList(int firstIndexOfList, int secondIndeOfList)
    {
        PuzzlePiece tempPiece = _pieces[firstIndexOfList];
        _pieces[firstIndexOfList] = _pieces[secondIndeOfList];
        _pieces[secondIndeOfList] = tempPiece;
    }

    private void UpdatePosition(int listIndex, Vector3 wallPosition)
    {
        _pieces[listIndex].piece.transform.localPosition = wallPosition;
    }

    private IEnumerator MoveToFront(PuzzlePiece currentPiece, float moveUntil)
    {
        _isMovingFirstPiece = true;

        _pieceDragAudio.Play();

        float startTime = Time.time;
        float elapsedTime = 0f;

        Vector3 startPosition = currentPiece.piece.transform.position;
        Vector3 endPosition = startPosition - Vector3.forward * moveUntil;

        while (elapsedTime < _moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);

            currentPiece.piece.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        currentPiece.piece.transform.position = endPosition;

        _isMovingFirstPiece = false;

        yield return null;
    }

    private async Task MoveToFront2(PuzzlePiece currentPiece, float moveUntil)
    {
        _isMovingFirstPiece = true;

        _pieceDragAudio.Play();

        float startTime = Time.time;
        float elapsedTime = 0f;

        Vector3 startPosition = currentPiece.piece.transform.position;
        Vector3 endPosition = startPosition - Vector3.forward * moveUntil;

        while (elapsedTime < _moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);

            currentPiece.piece.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            await Task.Yield();
        }

        currentPiece.piece.transform.position = endPosition;

        _isMovingFirstPiece = false;
    }

    private IEnumerator MoveToBack(PuzzlePiece currentPiece, float moveUntil)
    {
        _pieceDragAudio.Play();

        float startTime = Time.time;
        float elapsedTime = 0f;

        Vector3 startPosition = currentPiece.piece.transform.position;
        Vector3 endPosition = startPosition - Vector3.back * moveUntil;

        while (elapsedTime < _moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);

            currentPiece.piece.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        currentPiece.piece.transform.position = endPosition;

        yield return null;
    }

    private async Task MoveToBack2(PuzzlePiece currentPiece, float moveUntil)
    {
        _pieceDragAudio.Play();

        float startTime = Time.time;
        float elapsedTime = 0f;

        Vector3 startPosition = currentPiece.piece.transform.position;
        Vector3 endPosition = startPosition - Vector3.back * moveUntil;

        while (elapsedTime < _moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);

            currentPiece.piece.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            await Task.Yield();
        }

        currentPiece.piece.transform.position = endPosition;
    }

    /*
     * Mover a 2º peça até à 1º peça, mantendo o eixo z da 2º peça.
     */
    private IEnumerator MoveSecondToFirstPiece()
    {
        _pieceDragAudio.Play();

        float startTime = Time.time;
        float elapsedTime = 0f;

        Vector3 startPosition = _secondPiece.piece.transform.position;
        Vector3 endPosition = new Vector3(_firstPiece.piece.transform.position.x, _firstPiece.piece.transform.position.y, _secondPiece.piece.transform.position.z);

        while (elapsedTime < _moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);

            _secondPiece.piece.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        // garante que a 1º peça fique alinhada na 2º peça
        _secondPiece.piece.transform.position = endPosition;

        yield return null;
    }

    /*
     * Mover a 2º peça até à 1º peça, mantendo o eixo z da 2º peça.
     */
    private async Task MoveSecondToFirstPiece2()
    {
        _pieceDragAudio.Play();

        float startTime = Time.time;
        float elapsedTime = 0f;

        Vector3 startPosition = _secondPiece.piece.transform.position;
        Vector3 endPosition = new Vector3(_firstPiece.piece.transform.position.x, _firstPiece.piece.transform.position.y, _secondPiece.piece.transform.position.z);

        while (elapsedTime < _moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);

            _secondPiece.piece.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            await Task.Yield();
        }

        // garante que a 1º peça fique alinhada na 2º peça
        _secondPiece.piece.transform.position = endPosition;
    }

    /*
    * Mover a 1º peça até à 2º peça, mantendo o eixo z da 1º peça.
    */
    private IEnumerator MoveFirstToSecondPiece(Vector3 endPosition)
    {
        _pieceDragAudio.Play();

        float startTime = Time.time;
        float elapsedTime = 0f;

        Vector3 startPosition = _firstPiece.piece.transform.position;

        while (elapsedTime < _moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);

            _firstPiece.piece.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        // garante que a 1º peça fique alinhada na 2º peça
        _firstPiece.piece.transform.position = endPosition;

        yield return null;
    }

    /*
    * Mover a 1º peça até à 2º peça, mantendo o eixo z da 1º peça.
    */
    private async Task MoveFirstToSecondPiece2(Vector3 endPosition)
    {
        _pieceDragAudio.Play();

        float startTime = Time.time;
        float elapsedTime = 0f;

        Vector3 startPosition = _firstPiece.piece.transform.position;

        while (elapsedTime < _moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);

            _firstPiece.piece.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            await Task.Yield();
        }

        // garante que a 1º peça fique alinhada na 2º peça
        _firstPiece.piece.transform.position = endPosition;
    }

    /*
     * Limpar os valores do turno anterior (troca de 2 peças).
    */
    private void ResetValues()
    {
        _firstPiece = null;
        _secondPiece = null;
    }

    public bool CheckPuzzleSolved()
    {
        bool isSolved = true;

        for (int i = 0; i < _pieces.Count; i++)
        {
            if (_pieces[i].position != i + 1)
            {
                isSolved = false;
                break;
            }
        }

        return isSolved;
    }

    public void BeforeSolvePuzzle(GameObject playerCamera)
    {
        //Animator _animator = GetComponent<Animator>();
        //_animator.SetBool("isMoving", true);

        ThirdPersonCam thirdPersonCamera = playerCamera.GetComponent<ThirdPersonCam>();
        thirdPersonCamera.SwitchCameraStyle(ThirdPersonCam.CameraStyle.FocusOnPuzzle);

        GameObject playerPrefab = GameObject.FindGameObjectWithTag("PlayerPrefab");
        PlayerAnimations playerAnimations = playerPrefab.GetComponent<PlayerAnimations>();
        playerAnimations.FreezeAllAnimations = true;
        playerAnimations.StopAllAnimations();

        // executar o código se for a IA a jogar
        if (SceneManager.GetActiveScene().name == "SolvePuzzleAI")
        {
            if (Training)
            {
                Train();
            }
            else
            {
                InitialiseGame();
                GameStatusAI = GameStatusAI.WaitingOnHuman;
            }
        }

        _walkStarted = true;
    }

    public void AfterSolvePuzzle(GameObject playerCamera, ThirdPersonMovement playerScript)
    {
        _isSolving = false;

        ThirdPersonCam thirdPersonCamera = playerCamera.GetComponent<ThirdPersonCam>();
        thirdPersonCamera.SwitchCameraStyle(ThirdPersonCam.CameraStyle.Basic);

        _doorScript.StartMoving = true;
        _pyramidEntranceCollider.SetActive(true);

        playerScript.freeze = false;

        GameObject playerPrefab = GameObject.FindGameObjectWithTag("PlayerPrefab");
        PlayerAnimations playerAnimations = playerPrefab.GetComponent<PlayerAnimations>();
        playerAnimations.FreezeAllAnimations = false;
        playerAnimations.StopAllAnimations();

        Destroy(this);
    }

    /*
     * Caminhar automaticamente até à posição dos botões que movem o puzzle.
    */
    private void WalkToPuzzle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject playerPrefab = GameObject.FindGameObjectWithTag("PlayerPrefab");

        // animação de andar
        playerPrefab.GetComponent<Animator>().SetBool(Animations.WALKING, true);

        // calcula a direção para o ponto de destino
        Vector3 direction = _walkToPuzzlePoint.transform.position - playerPrefab.transform.position;
        direction.Normalize();

        // muda a rotação
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        playerPrefab.transform.rotation = lookRotation;

        // obtém o game object do player prefab para move-lo até aos botões do puzzle
        playerPrefab.transform.position += direction * 1.5f * Time.deltaTime;

        // verifica se o jogador chegou ao ponto de destino
        if (Vector3.Distance(playerPrefab.transform.position, _walkToPuzzlePoint.transform.position) < 0.1f)
        {
            _walkStarted = false;
            _lookStarted = true;

            playerPrefab.GetComponent<Animator>().SetBool(Animations.WALKING, false);
            player.GetComponent<ThirdPersonMovement>().freeze = true;
        }
    }

    /*
     * Olhar até à posição do puzzle na parede.
    */
    private void LookToPuzzle()
    {
        GameObject playerPrefab = GameObject.FindGameObjectWithTag("PlayerPrefab");

        Vector3 direction = _lookToPuzzlePoint.transform.position - playerPrefab.transform.position;
        direction.Normalize();

        // Calcula a rotação desejada
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

        // suaviza a transição de rotação
        float rotationSpeed = 2f;
        playerPrefab.transform.rotation = Quaternion.Slerp(playerPrefab.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        Vector3 forwardDirection = playerPrefab.transform.forward;

        float dotProduct = Vector3.Dot(direction, forwardDirection);

        // verifica se o jogador está a olhar para o puzzle
        float angleThreshold = 0.9f;
        if (dotProduct > angleThreshold)
        {
            _isSolving = true;
        }
    }



    #region INTELIGÊNCIA ARTIFICIAL PARA RESOLVE O PUZZLE

    //public float GameRunSpeed = 1f;

    public PlayerAI Player1;
    public PlayerAI Player2;

    public RewardsAI RewardsAI;

    //public bool Player1RandomFirstTurn = false;

    //public GameObject O;
    //public GameObject X;

    //public GameObject[] PiecePositions = new GameObject[9];

    public GameStatusAI GameStatusAI { get; set; }

    public GameResultAI GameResultAI { get; set; }

    public PlayerTypeAI CurrentPlayer { get; private set; }

    //public int HeuristicSelectedPiece { get; private set; }

    public bool AgentsWorking { get; private set; }

    public bool Training;

    private float Timeout { get; set; }

    public int[] BoardState { get; private set; }

    public int Turn { get; private set; }

    //private GameObject[] PlacedPieces;


    public void Train()
    {
        if (Player2.AgentStatusAI == AgentStatusAI.Ready && Player1.AgentStatusAI == AgentStatusAI.Ready)
        {
            Player2.AgentStatusAI = AgentStatusAI.Working;
            Player1.AgentStatusAI = AgentStatusAI.Working;

            InitialiseGame();
            AgentsWorking = true;
        }
    }

    private void InitialiseGame()
    {
        Timeout = 0;

        GameResultAI = GameResultAI.notSolved;

        if (Training)
        {
            GameStatusAI = GameStatusAI.ReadyToMove;
        }
        else
        {
            GameStatusAI = GameStatusAI.WaitingToStart;

            if (Player1.BehaviourParameters.BehaviorType == BehaviorType.HeuristicOnly)
            {
                if (Player1.BehaviourParameters.Model != null)
                {
                    Player1.BehaviourParameters.BehaviorType = BehaviorType.InferenceOnly;
                }
                else
                {
                    Debug.LogError("Nenhum modelo carregado para o player 1.");
                }
            }
            else
            {
                Player1.BehaviourParameters.BehaviorType = BehaviorType.HeuristicOnly;
            }

            if (Player2.BehaviourParameters.BehaviorType == BehaviorType.HeuristicOnly)
            {
                if (Player2.BehaviourParameters.Model != null)
                {
                    Player2.BehaviourParameters.BehaviorType = BehaviorType.InferenceOnly;
                }
                else
                {
                    Debug.LogError("Nenhum modelo carregado para o player 2.");
                }
            }
            else
            {
                Player2.BehaviourParameters.BehaviorType = BehaviorType.HeuristicOnly;
            }
        }

        BoardState = new int[9];

        CurrentPlayer = PlayerTypeAI.player1;

        Turn = 0;

        //HeuristicSelectedPiece = 0;

        //for (int i = 0; i < 9; i++)
        //{
        //    if (PlacedPieces != null && PlacedPieces[i] != null)
        //    {
        //        Destroy(PlacedPieces[i]);
        //    }
        //}

        //PlacedPieces = new GameObject[9];
    }

    //public bool CheckValidMove(int piece)
    //{
    //    piece -= 1;

    //    return BoardState[piece] == 0;
    //}

    //public bool[] GetAvailableMoves()
    //{
    //    bool[] availableGamePlayActions = new bool[10];

    //    for (int i = 0; i < 9; i++)
    //    {
    //        if (BoardState[i] == 0)
    //        {
    //            availableGamePlayActions[i + 1] = true;
    //        }
    //    }

    //    return availableGamePlayActions;
    //}

    //public int CheckCouldWinOnNextMove(PlayerTypeAI player)
    //{
    //    int couldWinOnNextMove = 0;

    //    int currentPlayer = 1;

    //    if (player == PlayerTypeAI.player2)
    //    {
    //        currentPlayer = 2;
    //    }

    //    int c;

    //    int[,] twoDArr = new int[3, 3];
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 3; j++)
    //        {
    //            twoDArr[i, j] = BoardState[i * 3 + j];
    //        }
    //    }

    //    for (int i = 0; i < 3; i++)
    //    {
    //        c = 0;

    //        c += PiecePlaceCount(twoDArr[i, 0], currentPlayer);
    //        c += PiecePlaceCount(twoDArr[i, 1], currentPlayer);
    //        c += PiecePlaceCount(twoDArr[i, 2], currentPlayer);

    //        if (c == 2)
    //        {
    //            couldWinOnNextMove += 1;
    //        }
    //    }

    //    for (int j = 0; j < 3; j++)
    //    {
    //        c = 0;

    //        c += PiecePlaceCount(twoDArr[0, j], currentPlayer);
    //        c += PiecePlaceCount(twoDArr[1, j], currentPlayer);
    //        c += PiecePlaceCount(twoDArr[2, j], currentPlayer);

    //        if (c == 2)
    //        {
    //            couldWinOnNextMove += 1;
    //        }
    //    }

    //    c = 0;

    //    c += PiecePlaceCount(twoDArr[0, 0], currentPlayer);
    //    c += PiecePlaceCount(twoDArr[1, 1], currentPlayer);
    //    c += PiecePlaceCount(twoDArr[2, 2], currentPlayer);

    //    if (c == 2)
    //    {
    //        couldWinOnNextMove += 1;
    //    }

    //    c = 0;

    //    c += PiecePlaceCount(twoDArr[0, 2], currentPlayer);
    //    c += PiecePlaceCount(twoDArr[1, 1], currentPlayer);
    //    c += PiecePlaceCount(twoDArr[2, 0], currentPlayer);

    //    if (c == 2)
    //    {
    //        couldWinOnNextMove += 1;
    //    }

    //    return couldWinOnNextMove;
    //}

    //private int PiecePlaceCount(int piecePlace, int player)
    //{
    //    if (piecePlace == 0)
    //    {
    //        return 0;
    //    }
    //    else if (piecePlace == player)
    //    {
    //        return 1;
    //    }
    //    else
    //    {
    //        return -1;
    //    }
    //}

    public async Task<bool> PlacePiece(int piece)
    {
        if (!CheckValidPlay(piece))
        //if (!CheckValidMove(piece))
        {
            return false;
        }

        Debug.Log("Numero:" + piece);

        if (CurrentPlayer == PlayerTypeAI.player1)
        {
            //PlacedPieces[piece] = Instantiate(X, PiecePositions[piece].transform.position, Quaternion.identity);

            _firstPiece = ChoosePiece(piece);
            await MoveToFront2(_firstPiece, _firstPieceToFrontDistance);

            BoardState[piece - 1] = 1;
        }
        else if (CurrentPlayer == PlayerTypeAI.player2)
        {
            //PlacedPieces[piece] = Instantiate(O, PiecePositions[piece].transform.position, Quaternion.identity);

            _secondPiece = ChoosePiece(piece);

            _isMovingSecondPiece = true;

            Vector3 endStartPosition = new Vector3(_secondPiece.piece.transform.position.x, _secondPiece.piece.transform.position.y, _secondPiece.piece.transform.position.z);

            if (IsSamePiece())
            {
                await MoveToBack2(_firstPiece, _firstPieceToFrontDistance);
            }
            else
            {
                await MoveToFront2(_secondPiece, _secondPieceToFrontDistance);
                await MoveSecondToFirstPiece2();
                await MoveToBack2(_secondPiece, _secondPieceToFrontDistance);
                await MoveFirstToSecondPiece2(endStartPosition);

                int firstIndexOfList = _pieces.IndexOf(_firstPiece);
                int secondIndeOfList = _pieces.IndexOf(_secondPiece);
                SwapPieceInList(firstIndexOfList, secondIndeOfList);
            }

            ResetValues();

            _isMovingSecondPiece = false;

            BoardState[piece - 1] = 2;
        }

        return true;
    }

    private void ChangePlayer()
    {
        Turn += 1;
        if (CurrentPlayer == PlayerTypeAI.player1)
        {
            CurrentPlayer = PlayerTypeAI.player2;
        }
        else
        {
            CurrentPlayer = PlayerTypeAI.player1;
        }

        if (Training)
        {
            GameStatusAI = GameStatusAI.ReadyToMove;
        }
        else
        {
            GameStatusAI = GameStatusAI.WaitingOnHuman;
        }
    }

    public GameResultAI CheckGameStatusAI()
    {
        bool isSolved = CheckPuzzleSolved();

        if (isSolved)
        {
            return GameResultAI.solved;
        }
        else
        {
            return GameResultAI.notSolved;
        }

        //int winner = 0;

        //int[,] twoDArr = new int[3, 3];
        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        twoDArr[i, j] = BoardState[i * 3 + j];
        //    }
        //}

        //for (int i = 0; i < 3; i++)
        //{
        //    if (twoDArr[i, 0] == twoDArr[i, 1] && twoDArr[i, 1] == twoDArr[i, 2])
        //    {
        //        if (twoDArr[i, 0] != 0)
        //        {
        //            winner = twoDArr[i, 0];
        //        }
        //    }
        //}

        //for (int j = 0; j < 3; j++)
        //{
        //    if (twoDArr[0, j] == twoDArr[1, j] && twoDArr[1, j] == twoDArr[2, j])
        //    {
        //        if (twoDArr[0, j] != 0)
        //        {
        //            winner = twoDArr[0, j];
        //        }
        //    }
        //}

        //if (twoDArr[0, 0] == twoDArr[1, 1] && twoDArr[1, 1] == twoDArr[2, 2])
        //{
        //    if (twoDArr[0, 0] != 0)
        //    {
        //        winner = twoDArr[0, 0];
        //    }
        //}
        //if (twoDArr[0, 2] == twoDArr[1, 1] && twoDArr[1, 1] == twoDArr[2, 0])
        //{
        //    if (twoDArr[0, 2] != 0)
        //    {
        //        winner = twoDArr[0, 2];
        //    }
        //}

        //if (winner == 1)
        //{
        //    return GameResultAI.xWon;
        //}
        //else if (winner == 2)
        //{
        //    return GameResultAI.oWon;
        //}
        //else if (winner == 0 && Turn == 8)
        //{
        //    return GameResultAI.draw;
        //}
        //else
        //{
        //    return GameResultAI.none;
        //}
    }

    private void RequestDecision(PlayerAI player)
    {
        player.RequestDecision();
    }

    public void GiveRewards()
    {
        if (GameResultAI == GameResultAI.solved)
        {
            Player1.AddReward(RewardsAI.HAS_PUZZLE_SOLVED);
            Player2.AddReward(RewardsAI.HAS_PUZZLE_SOLVED);
        }
        else if (GameResultAI == GameResultAI.notSolved)
        {
            Player1.AddReward(RewardsAI.NOT_HAS_PUZZLE_SOLVED);
            Player2.AddReward(RewardsAI.NOT_HAS_PUZZLE_SOLVED);
        }
        //else if (GameResultAI == GameResultAI.xWon)
        //{
        //    Player1.AddReward(Rewards.Player1Win);
        //    Player2.AddReward(Rewards.Player2Lost);
        //}
        //else if (GameResultAI == GameResultAI.oWon)
        //{
        //    Player2.AddReward(Rewards.Player2Win);
        //    Player1.AddReward(Rewards.Player1Lost);
        //}
        //else
        //{
        //    Player1.AddReward(Rewards.Player1Draw);
        //    Player2.AddReward(Rewards.Player2Draw);
        //}

        GameStatusAI = GameStatusAI.FinalObservation;
    }

    //public void MakeAIMove()
    //{
    //    GameStatusAI = GameStatusAI.ReadyToMove;
    //}

    //public void MakeHeuristicMove(int piece)
    //{
    //    piece += PieceOffset;

    //    if (CheckValidMove(piece))
    //    {
    //HeuristicSelectedPiece = piece;
    //        GameStatusAI = GameStatusAI.ReadyToMove;
    //    }
    //}

    public void EndGame()
    {
        Player1.EndEpisode();
        Player2.EndEpisode();
    }

    public void ResetGame()
    {
        AgentsWorking = false;

        Player2.EpisodeInterrupted();
        Player1.EpisodeInterrupted();
    }

    //public void StartHumanGame()
    //{
    //    InitialiseGame();
    //    GameStatusAI = GameStatusAI.WaitingOnHuman;
    //}

    #endregion
}