using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    /* ATRIBUTOS */

    private static GameManager _instance;

    [SerializeField] private GameObject _player;

    [SerializeField] public ReactiveProperty<GameState> _currentGameState = new();
    private List<MapAction> _currentMapActions = new();

    [SerializeField] private List<GameStateInfo> _gameStateList;
    [SerializeField] private List<MapAction> _mapActions;

    [SerializeField] private float _actionButtonsVisibilityDistance = 20f;
    [SerializeField] private float _actionButtonsClickDistance = 2f;

    [SerializeField] private Canvas _canvas;

    private bool _isChangingPositon;
    private Vector3 positionToChange;
    private Vector3 rotationToChange;

    [SerializeField] public Camera _playerCamera;

    private Vector3 _lastCheckPointPos;
    private ThirdPersonMovement _playerScript;

    [SerializeField] private GameObject _puzzleManagerObject;
    private PuzzleManager _puzzleManagerScript;


    /* PROPRIEDADES */

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    public IReadOnlyReactiveProperty<GameState> CurrentGameState => _currentGameState;

    public List<MapAction> CurrentMapActions
    {
        get { return _currentMapActions; }
        set { _currentMapActions = value; }
    }

    public Vector3 LastCheckPointPos
    {
        get { return _lastCheckPointPos; }
        set { _lastCheckPointPos = value; }
    }


    /* M�TODOS */

    /*
     * Garante apenas uma inst�ncia de GameManager por cena.
    */
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HideAllActionButtons();

        // se a cena inicializada � da caverna e o estado atual � GO_TO_CAVE,
        // quer dizer que passou da cena da floresta para a caverna
        if (scene.name == "CaveAndPyramid" && CurrentGameState.Value == GameState.GO_TO_CAVE)
        {
            // reatribui as inst�ncias dos game objects do Game Manager da nova cena
            _currentMapActions.Clear();
            _player = GameObject.Find("Player");
            _canvas = FindObjectOfType<Canvas>();

            VideoPlayer introCaveCutscene = GameObject.Find("IntroCave").GetComponent<VideoPlayer>();
            _gameStateList[7].cutscene = introCaveCutscene;
            _mapActions[16].gameStateInfo.cutscene = introCaveCutscene;

            ChangeGameState(GameState.INTRO_CAVE);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /*
     * Esconde todos os bot�es de a��o no mapa por padr�o.
     * Observa o estado atual do jogo, para que sempre que haja uma mudan�a o evento seja acionado.
    */
    private void Start()
    {
        _playerScript = _player.GetComponent<ThirdPersonMovement>();

        // assina o observ�vel para detetar mudan�as de estado
        _currentGameState.Subscribe(gameState =>
        {
            HandleGameStateChange(gameState);
        });
    }

    private void Update()
    {
        // bloqueia outras a��es quando est� a resolver o puzzle
        if (_puzzleManagerScript != null)
        {
            if (_puzzleManagerScript.IsSolving)
            {
                if (_puzzleManagerScript.CheckPuzzleSolved())
                {
                    _puzzleManagerScript.AfterSolvePuzzle();
                }

                _puzzleManagerScript.DoPlay();
            }

            return;
        }

        if (_player == null)
        {
            return;
        }

        if (_playerScript.IsDead)
        {
            Invoke(nameof(RestartGame), 4);
            return;
        }

        // n�o h� necessidade de verificar se clicou no bot�o de a��o,
        // quando o estado � apenas de mostrar uma cutscene
        if (_currentGameState.Value != GameState.INTRO_GAME ||
        _currentGameState.Value != GameState.INTRO_FOREST ||
        _currentGameState.Value != GameState.INTRO_CAMP ||
        _currentGameState.Value != GameState.INTRO_CAVE)
        {
            CheckActionButtonsVisibilityDistance();
            CheckActionButtonsClickDistance();
        }
    }

    private void FixedUpdate()
    {
        if (_isChangingPositon)
        {
            ChangePlayerPosition(positionToChange, rotationToChange);
            _isChangingPositon = false;
        }
    }

    /*
     * Trata da mudan�a para os diferentes estados do jogo.
     * _currentMapActions[0] - porque existe sempre pelo menos uma a��o do mapa associada a um game state
    */
    private void HandleGameStateChange(GameState nextGameState)
    {
        HideCurrentActionButtons();

        _currentGameState.Value = nextGameState;
        _currentMapActions = GetCurrentMapActions();

        if (_currentMapActions[0].gameStateInfo.hasNewPosition)
        {
            positionToChange = _currentMapActions[0].gameStateInfo.position;
            rotationToChange = _currentMapActions[0].gameStateInfo.rotation;
            _isChangingPositon = true;
        }

        // configura��es espec�ficas na mudan�a de estado
        switch (nextGameState)
        {
            // mostra a cutscene e trata do colisor no script LevelChanger
            case GameState.INTRO_GAME:
            case GameState.INTRO_FOREST:
            case GameState.INTRO_CAMP:
            case GameState.INTRO_CAVE:
            case GameState.INTRO_PYRAMID:
                ConfigCutscene(nextGameState);
                break;

            case GameState.SOLVE_PUZZLE:
                _puzzleManagerScript = _puzzleManagerObject.GetComponent<PuzzleManager>();
                _puzzleManagerScript.IsSolving = true;
                break;

            default:
                break;
        }
    }

    /*
     * Obt�m as a��es atuais do mapa associadas ao estado de jogo atual.
    */
    private List<MapAction> GetCurrentMapActions()
    {
        _currentMapActions.Clear();

        foreach (MapAction mapAction in _mapActions)
        {
            if (mapAction.gameStateInfo.gameState == _currentGameState.Value)
            {
                _currentMapActions.Add(mapAction);
            }
        }

        return _currentMapActions;
    }

    /*
     * Procura a a��o atual que diz respeito ao objetivo,
     * que ser� a que tem "hasProgress" como true e devolve o t�tulo.
    */
    public string GetCurrentGoal()
    {
        foreach (MapAction mapAction in GameManager.Instance._currentMapActions)
        {
            if (mapAction.hasProgress)
            {
                return mapAction.title;
            }
        }

        return "";
    }

    private GameState GetNextGameState(GameState currentGameState)
    {
        int nextGameStateNumber = (int)currentGameState + 1;
        return (GameState)nextGameStateNumber;
    }

    private int GetLastGameStateInfoIndex()
    {
        int lastGameStateInfoIndex = 0;

        for (int i = 0; i < _gameStateList.Count; i++)
        {
            if (_currentGameState.Value == _gameStateList[i].gameState)
            {
                lastGameStateInfoIndex = i;
            }
        }

        return lastGameStateInfoIndex;
    }

    public void SetGameState(GameState newGameState)
    {
        _currentGameState.Value = newGameState;
    }

    private void ConfigCutscene(GameState nextGameState)
    {
        OnCutsceneStart(_currentMapActions[0].gameStateInfo.cutscene);

        nextGameState = GetNextGameState(_currentGameState.Value);

        // evento de t�rmino da cutscene
        _currentMapActions[0].gameStateInfo.cutscene.loopPointReached += (videoPlayer) => OnCutsceneEnd(_currentMapActions[0].gameStateInfo.cutscene, nextGameState);
    }

    private void OnCutsceneStart(VideoPlayer videoPlayer)
    {
        Time.timeScale = 0f;

        _canvas.enabled = false;
        videoPlayer.enabled = true;
        videoPlayer.Play();
    }

    private void OnCutsceneEnd(VideoPlayer videoPlayer, GameState nextGameState)
    {
        _canvas.enabled = true;
        videoPlayer.enabled = false;

        Time.timeScale = 1f;

        if (_currentMapActions[0].gameStateInfo.hasNewPosition)
        {
            int lastGameStateInfoIndex = GetLastGameStateInfoIndex();
            positionToChange = _gameStateList[lastGameStateInfoIndex].position;
            rotationToChange = _gameStateList[lastGameStateInfoIndex].rotation;

            _isChangingPositon = true;
        }

        ChangeGameState(nextGameState);
    }

    private void ChangeGameState(GameState newGameState)
    {
        _currentGameState.Value = newGameState;
    }

    public void ChangePlayerPosition(Vector3 positon, Vector3 rotation)
    {
        _player.transform.localPosition = positon;
        _player.transform.localRotation = Quaternion.Euler(rotation);
    }

    /*
     * Ao iniciar a cena, todos os bot�es de a��o s�o ocultos por padr�o.
    */
    private void HideAllActionButtons()
    {
        foreach (MapAction mapAction in _mapActions)
        {
            if (mapAction.button != null)
            {
                mapAction.button.SetActive(false);
            }
        }
    }

    /*
     * Esconde os bot�es das a��es do estado atual.
     * Utiliz�vel quando o utilizador transita para o pr�ximo estado de jogo e as a��es do estado anterior j� n�o interessam.
    */
    private void HideCurrentActionButtons()
    {
        foreach (MapAction mapAction in _currentMapActions)
        {
            if (mapAction.hasClick)
            {
                mapAction.button.SetActive(false);
            }
        }
    }

    /*
     * Se o jogador estiver perto das a��es de jogo, o bot�o de a��o ser� vis�vel, caso contr�rio continuar� oculto.
    */
    private void CheckActionButtonsVisibilityDistance()
    {
        foreach (MapAction mapAction in _currentMapActions)
        {
            if (mapAction.hasClick)
            {
                if (_actionButtonsVisibilityDistance >= Utils.GetDistanceBetween2Objects(_player, mapAction.button))
                {
                    mapAction.button.SetActive(true);
                    CenterActionButtonInCamera(mapAction.button);
                }
            }
        }
    }

    /*
     * Se o jogador estiver muito perto das a��es de jogo, o bot�o de a��o poder� ser clicado, caso contr�rio n�o.
    */
    private void CheckActionButtonsClickDistance()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (MapAction mapAction in _currentMapActions)
            {
                if (mapAction.hasClick)
                {
                    if (_actionButtonsClickDistance >= Utils.GetDistanceBetween2Objects(_player, mapAction.button))
                    {
                        // aciona eventos
                        ProgressActionEvent(mapAction);
                        NoProgressActionEvent(mapAction);
                        return;
                    }
                }
            }
        }
    }

    /*
     * Quando a��o clicada permite que o jogador progrida no jogo (passe para o pr�ximo game state).
     * Quando o jogador alcan�a um objetivo, este n�o pode voltar ao objetivo anterior.
     * Exemplo: Quando encontra o esconderijo para a nave.
    */
    private void ProgressActionEvent(MapAction mapAction)
    {
        if (mapAction.hasProgress)
        {
            if (mapAction.hasClick)
            {
                mapAction.button.SetActive(false);
                mapAction.hasClick = false;
            }

            if (_currentGameState.Value == GameState.GO_TO_FOREST ||
                _currentGameState.Value == GameState.GO_TO_PYRAMID)
            {
                GameState nextGameState = GetNextGameState(_currentGameState.Value);
                ChangeGameState(nextGameState);
            }

            if (mapAction.gameStateInfo.hasCutscene)
            {
                OnCutsceneStart(mapAction.gameStateInfo.cutscene);

                GameState nextGameState = GetNextGameState(_currentGameState.Value);

                // evento de t�rmino da cutscene
                mapAction.gameStateInfo.cutscene.loopPointReached += (videoPlayer) => OnCutsceneEnd(mapAction.gameStateInfo.cutscene, nextGameState);
            }
        }
    }

    /*
     * Quando a��o clicada n�o permite que o jogador progrida no jogo (passe para o pr�ximo game state).
     * Exemplo: Quando o jogador tenta um caminho errado para a floresta. Quando observa uma pista na floresta.
    */
    private void NoProgressActionEvent(MapAction mapAction)
    {
        if (!mapAction.hasProgress)
        {
            if (mapAction.hasDialogue)
            {
                Debug.Log("A��o sem progress�o! Halley deve falar algo!");
            }
        }
    }

    private void CenterActionButtonInCamera(GameObject actionButton)
    {
        Vector3 cameraPosition = _playerCamera.transform.position;
        Vector3 objectPosition = actionButton.transform.position;
        Vector3 direction = cameraPosition - objectPosition;

        actionButton.transform.rotation = Quaternion.LookRotation(direction);
    }

    private void RestartGame()
    {
        if (_lastCheckPointPos != null) _player.transform.position = _lastCheckPointPos;
        _playerScript.IsDead = false;
        _playerScript.HealthManager.restoreHealth();
        CancelInvoke(nameof(RestartGame));
    }
}