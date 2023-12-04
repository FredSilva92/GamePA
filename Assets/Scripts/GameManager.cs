using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
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

    [SerializeField] private AudioSource _audioSource;

    private bool _isChangingPositon = false;
    private Vector3 positionToChange;
    private Vector3 rotationToChange;

    [SerializeField] public Camera _playerCamera;

    private Vector3 _lastCheckPointPos;
    private ThirdPersonMovement _playerScript;

    [SerializeField] private GameObject _puzzleManagerObject;
    private PuzzleManager _puzzleManagerScript;

    [SerializeField] private GameObject _currentGoalPanel;
    [SerializeField] private TextMeshProUGUI _currentGoalTextMeshPro;

    [SerializeField] private GameObject _currentActionPanel;
    [SerializeField] private TextMeshProUGUI _currentActionTextMeshPro;

    [SerializeField] private GameObject _starship;


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
    }

    /*
     * Esconde todos os bot�es de a��o no mapa por padr�o.
     * Observa o estado atual do jogo, para que sempre que haja uma mudan�a o evento seja acionado.
    */
    private void Start()
    {
        HideCurrentActionLabel();
        HideAllActionButtons();

        InvokeRepeating(nameof(ShowAndHideGoalLoop), 4f, 30f);

        _playerScript = _player.GetComponent<ThirdPersonMovement>();

        // assina o observ�vel para detetar mudan�as de estado
        _currentGameState.Subscribe(gameState =>
        {
            HandleGameStateChange(gameState);
        });
    }

    private void Update()
    {
        if (_player == null)
        {
            return;
        }

        if (_playerScript.IsDead)
        {
            Invoke(nameof(RestartGame), 4);
            return;
        }

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

        // n�o h� necessidade de verificar se clicou no bot�o de a��o,
        // quando o estado � apenas de mostrar uma cutscene
        if (_currentGameState.Value != GameState.INTRO_GAME ||
            _currentGameState.Value != GameState.INTRO_FOREST ||
            _currentGameState.Value != GameState.INTRO_CAMP ||
            _currentGameState.Value != GameState.INTRO_CAVE ||
            _currentGameState.Value != GameState.INTRO_PYRAMID)
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
            // mostra a cutscene externa e trata do colisor no script LevelChanger
            case GameState.INTRO_GAME:
            case GameState.FINISH_GAME:
            case GameState.INTRO_FOREST:
            case GameState.INTRO_CAVE:
            case GameState.INTRO_PYRAMID:
                ConfigVideoCutscene(nextGameState);
                break;

            // mostra a cutscene dentro do unity e trata do colisor no script LevelChanger
            //case GameState.INTRO_FOREST:
            case GameState.INTRO_CAMP:
                //case GameState.INTRO_CAVE:
                //case GameState.INTRO_PYRAMID:
                ConfigTimelineCutscene(nextGameState);
                break;

            // muda a posi??o da nave na praia
            case GameState.GO_TO_FOREST:
                _starship.SetActive(true);
                _starship.transform.localPosition = new Vector3(-19.17f, -4f, 65.87f);
                _starship.transform.localRotation = Quaternion.Euler(2.002f, -27.307f, -1.41f);
                break;

            // permite que o jogador comece a resolver o puzzle
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

    private void ConfigVideoCutscene(GameState nextGameState)
    {
        OnVideoCutsceneStart(_currentMapActions[0].gameStateInfo.videoCutscene);

        nextGameState = GetNextGameState(_currentGameState.Value);

        // evento de t�rmino da cutscene
        _currentMapActions[0].gameStateInfo.videoCutscene.loopPointReached += (videoPlayer) => OnVideoCutsceneEnd(_currentMapActions[0].gameStateInfo.videoCutscene, nextGameState);
    }

    private void ConfigTimelineCutscene(GameState nextGameState)
    {
        _player.SetActive(false);

        GameObject timelineObject = _currentMapActions[0].gameStateInfo.timelineCutscene;
        PlayableDirector timeline = timelineObject.GetComponent<PlayableDirector>();

        timeline.played += (timelineObject) => OnTimelineCutsceneStart(timeline, _currentMapActions[0].gameStateInfo.timelineCutscene);
        OnTimelineCutsceneStart(timeline, timelineObject);

        nextGameState = GetNextGameState(_currentGameState.Value);

        // evento de t�rmino da cutscene
        timeline.stopped += (timeline) => OnTimelineCutsceneEnd(timeline, timelineObject, nextGameState);
    }

    private void OnVideoCutsceneStart(VideoPlayer videoPlayer)
    {
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.Pause();
        }

        Time.timeScale = 0f;

        _canvas.enabled = false;
        videoPlayer.enabled = true;
        videoPlayer.Play();
    }

    private void OnVideoCutsceneEnd(VideoPlayer videoPlayer, GameState nextGameState)
    {
        if (_audioSource != null && !_audioSource.isPlaying)
        {
            _audioSource.UnPause();
        }

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

        if (_currentGameState.Value == GameState.FINISH_GAME)
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
            return;
        }

        ChangeGameState(nextGameState);
    }

    private void OnTimelineCutsceneStart(PlayableDirector timeline, GameObject timelineObject)
    {
        _player.SetActive(false);

        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.Pause();
        }

        _canvas.enabled = false;

        timelineObject.SetActive(true);
        timeline.Play();
    }

    private void OnTimelineCutsceneEnd(PlayableDirector timeline, GameObject timelineObject, GameState nextGameState)
    {
        if (_audioSource != null && !_audioSource.isPlaying)
        {
            _audioSource.UnPause();
        }

        _canvas.enabled = true;
        timelineObject.SetActive(false);

        if (_currentMapActions[0].gameStateInfo.hasNewPosition)
        {
            int lastGameStateInfoIndex = GetLastGameStateInfoIndex();
            positionToChange = _gameStateList[lastGameStateInfoIndex].position;
            rotationToChange = _gameStateList[lastGameStateInfoIndex].rotation;

            _isChangingPositon = true;
        }

        ChangeGameState(nextGameState);

        _player.SetActive(true);
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

    private void HideCurrentActionLabel()
    {
        _currentActionPanel.SetActive(false);
    }

    private void HideCurrentGoalLabel()
    {
        _currentGoalPanel.SetActive(false);
    }

    /*
     * Mostra o texto da a��o atual e fala, apenas por 4 segundos e depois volta a desaparecer
    */
    private IEnumerator ShowAndHideActionLabel(string text, GameObject dialogueObject)
    {
        AudioSource dialogueSource = dialogueObject.GetComponent<AudioSource>();
        float dialogueDuration = dialogueSource.clip.length;

        dialogueSource.Play();

        _currentActionTextMeshPro.text = text;
        _currentActionPanel.SetActive(true);

        yield return new WaitForSeconds(dialogueDuration + 1f);

        _currentActionPanel.SetActive(false);
    }

    private void ShowAndHideGoalLoop()
    {
        StartCoroutine(ShowAndHideGoalLabel());
    }

    /*
     * Mostra o texto do objetivo no canto superior direito apenas por 4 segundos a cada 20 segundos e depois volta a desaparecer.
    */
    private IEnumerator ShowAndHideGoalLabel()
    {
        foreach (MapAction mapAction in _currentMapActions)
        {
            if (mapAction.hasProgress)
            {
                _currentGoalTextMeshPro.text = mapAction.title;
            }
        }

        _currentGoalPanel.SetActive(true);

        yield return new WaitForSeconds(7f);

        _currentGoalPanel.SetActive(false);
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
                _currentGameState.Value == GameState.GO_TO_PYRAMID ||
                _currentGameState.Value == GameState.PICK_TREASURE)
            {
                GameState nextGameState = GetNextGameState(_currentGameState.Value);
                ChangeGameState(nextGameState);
            }

            if (mapAction.gameStateInfo.hasCutscene)
            {
                if (mapAction.gameStateInfo.cutsceneType == CutsceneType.EXTERNAL)
                {
                    OnVideoCutsceneStart(mapAction.gameStateInfo.videoCutscene);

                    GameState nextGameState = GetNextGameState(_currentGameState.Value);

                    // evento de t�rmino da cutscene
                    mapAction.gameStateInfo.videoCutscene.loopPointReached += (videoPlayer) => OnVideoCutsceneEnd(mapAction.gameStateInfo.videoCutscene, nextGameState);
                }
                else if (mapAction.gameStateInfo.cutsceneType == CutsceneType.INSIDE_EDITOR)
                {
                    if (_currentGameState.Value == GameState.HIDE_SHIP)
                    {
                        _starship.SetActive(false);
                    }

                    GameObject timelineObject = mapAction.gameStateInfo.timelineCutscene;
                    PlayableDirector timeline = timelineObject.GetComponent<PlayableDirector>();

                    timeline.played += (timelineObject) => OnTimelineCutsceneStart(timeline, mapAction.gameStateInfo.timelineCutscene);
                    OnTimelineCutsceneStart(timeline, timelineObject);

                    GameState nextGameState = GetNextGameState(_currentGameState.Value);

                    // evento de t�rmino da cutscene
                    timeline.stopped += (timeline) => OnTimelineCutsceneEnd(timeline, timelineObject, nextGameState);
                }
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
                StartCoroutine(ShowAndHideActionLabel(mapAction.title, mapAction.dialogue));
            }
        }
    }

    private void CenterActionButtonInCamera(GameObject actionButton)
    {
        Vector3 cameraPosition = _playerCamera.transform.position;
        Vector3 objectPosition = actionButton.transform.position;
        Vector3 direction = cameraPosition - objectPosition;

        actionButton.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
    }

    private void RestartGame()
    {
        if (_lastCheckPointPos != null) _player.transform.position = _lastCheckPointPos;
        _playerScript.IsDead = false;
        _playerScript.HealthManager.restoreHealth();
        CancelInvoke(nameof(RestartGame));
    }
}