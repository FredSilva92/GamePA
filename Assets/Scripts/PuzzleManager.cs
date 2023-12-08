using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utils;

public class PuzzleManager : MonoBehaviour
{
    /* ATRIBUTOS */

    [SerializeField] private int[] pieceOrder = { 6, 9, 8, 7, 2, 4, 5, 3, 1 };

    [SerializeField] private List<Transform> _wallPoints;
    [SerializeField] private List<PuzzlePiece> _puzzlePieces;

    private PuzzlePiece _firstPiece = null;
    private PuzzlePiece _secondPiece = null;
    private int _firstPositionChosen = -1;
    private int _secondPositionChosen = -1;

    private bool _isSolving = false;

    [SerializeField] private GameObject _door;
    private DoorAnimationManager _doorScript;

    [SerializeField] private GameObject _pyramidEntranceCollider;

    [SerializeField] private GameObject _walkToPuzzlePoint;
    [SerializeField] private GameObject _lookToPuzzlePoint;

    private bool _walkStarted = false;
    private bool _lookStarted = false;


    /* PROPRIEDADES */

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
    }

    private void ShufflePuzzle()
    {
        _puzzlePieces = _puzzlePieces.OrderBy(puzzlePiece => Array.IndexOf(pieceOrder, puzzlePiece.position)).ToList();

        for (int i = 0; i < _puzzlePieces.Count; i++)
        {
            Vector3 newPosition = _wallPoints[i].localPosition;
            UpdatePosition(i, newPosition);
        }
    }

    private void UpdatePosition(int listIndex, Vector3 wallPosition)
    {
        _puzzlePieces[listIndex].piece.transform.localPosition = wallPosition;
    }

    public void DoPlay()
    {
        // se uma tecla for pressionada
        if (Input.anyKeyDown)
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
                    _firstPositionChosen = inputtedNumber;
                    return;
                }
                else
                {
                    _secondPiece = ChoosePiece(inputtedNumber);
                    _secondPositionChosen = inputtedNumber;
                    MovePieces();
                    ResetValues();
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

    private PuzzlePiece ChoosePiece(int inputtedNumber)
    {
        PuzzlePiece puzzlePiece = _puzzlePieces[inputtedNumber - 1];

        if (puzzlePiece != null)
        {
            return puzzlePiece;
        }
        else
        {
            return null;
        }
    }

    private void MovePieces()
    {
        int firstIndex = _puzzlePieces.IndexOf(_firstPiece);
        int secondIndex = _puzzlePieces.IndexOf(_secondPiece);

        SwapPuzzlePiecePosition(firstIndex, secondIndex);
        SwapPuzzlePieceInList(firstIndex, secondIndex);
    }

    private void SwapPuzzlePiecePosition(int firstIndex, int secondIndex)
    {
        Vector3 firstWallPosition = _wallPoints[_firstPositionChosen - 1].localPosition;
        Vector3 secondWallPosition = _wallPoints[_secondPositionChosen - 1].localPosition;

        UpdatePosition(firstIndex, secondWallPosition);
        UpdatePosition(secondIndex, firstWallPosition);
    }

    private void SwapPuzzlePieceInList(int firstIndex, int secondIndex)
    {
        PuzzlePiece tempPuzzlePiece = _puzzlePieces[firstIndex];
        _puzzlePieces[firstIndex] = _puzzlePieces[secondIndex];
        _puzzlePieces[secondIndex] = tempPuzzlePiece;
    }

    /*
     * Limpar os valores do turno anterior (troca de 2 peças).
    */
    private void ResetValues()
    {
        _firstPiece = null;
        _secondPiece = null;

        _firstPositionChosen = -1;
        _secondPositionChosen = -1;
    }

    public bool CheckPuzzleSolved()
    {
        bool isSolved = true;

        for (int i = 0; i < _puzzlePieces.Count; i++)
        {
            if (_puzzlePieces[i].position != i + 1)
            {
                isSolved = false;
                break;
            }
        }

        return isSolved;
    }

    public void BeforeSolvePuzzle(GameObject playerCamera, ThirdPersonMovement playerScript)
    {
        ThirdPersonCam thirdPersonCamera = playerCamera.GetComponent<ThirdPersonCam>();
        thirdPersonCamera.SwitchCameraStyle(ThirdPersonCam.CameraStyle.FocusOnPuzzle);

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

        Destroy(this);
    }

    /*
     * Caminhar automaticamente até à posição dos botões que movem o puzzle.
    */
    private void WalkToPuzzle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject playerPrefab = GameObject.FindGameObjectWithTag("PlayerPrefab");

        PlayerAnimations playerAnimations = playerPrefab.GetComponent<PlayerAnimations>();
        playerAnimations.FreezeAllAnimations = true;
        playerPrefab.GetComponent<Animator>().SetBool(Animations.WALKING, true);

        // calcula a direção para o ponto de destino
        Vector3 direction = _walkToPuzzlePoint.transform.position - playerPrefab.transform.position;
        direction.Normalize();

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
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
}