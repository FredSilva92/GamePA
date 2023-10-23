using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    /* ATRIBUTOS */

    private static GameManager _instance;

    [SerializeField] private GameObject _player;

    [SerializeField] private GameState _currentGameState;

    [SerializeField] private List<GameStateInfo> _gameStateList;
    [SerializeField] private List<MapAction> _mapActions;

    private List<MapAction> _currentMapActions = new List<MapAction>();

    [SerializeField] private float _actionButtonsVisibilityDistance;
    [SerializeField] private float _actionButtonsClickDistance;

    [SerializeField] private Canvas _canvas;


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


    /* M�TODOS */

    /*
     * Permite apenas uma inst�ncia de GameManager por cena.
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

    private void Start()
    {
        _currentMapActions = GetCurrentMapActions();

        if (_currentGameState == GameState.INTRO_TO_GAME)
        {
            OnCutsceneStart(_currentMapActions[0].gameStateInfo.cutscene);

            // evento de t�rmino da cutscene
            _currentMapActions[0].gameStateInfo.cutscene.loopPointReached += (videoPlayer) => OnCutsceneEnd(_currentMapActions[0].gameStateInfo.cutscene, GameState.HIDE_SHIP);
        }

        HideAllActionButtons();
    }

    private void Update()
    {
        if (_currentGameState != GameState.INTRO_TO_GAME)
        {
            CheckActionButtonsVisibilityDistance();
            CheckActionButtonsClickDistance();
        }

        if (_currentGameState == GameState.HIDE_SHIP)
        {

        }
    }

    private List<MapAction> GetCurrentMapActions()
    {
        _currentMapActions.Clear();

        foreach (MapAction mapAction in _mapActions)
        {
            if (mapAction.gameStateInfo.gameState == _currentGameState)
            {
                _currentMapActions.Add(mapAction);
            }
        }

        return _currentMapActions;
    }

    /*
     * Procura a a��o atual que diz respeito ao objetivo, que ser� a que tem "hasProgress" como true,
     * e devolve o t�tulo.
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
        TransitionGameState(nextGameState);
    }

    /*
     * Altera o estado do jogo e atualiza a lista de a��es do mapa atuais.
    */
    private void TransitionGameState(GameState nextGameState)
    {
        HideCurrentActionButtons();

        _currentGameState = nextGameState;
        _currentMapActions = GetCurrentMapActions();
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
            if (mapAction.button != null)
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
        foreach (MapAction mapAction in _mapActions)
        {
            if (mapAction.hasClick && _actionButtonsVisibilityDistance >= Utils.GetDistanceBetween2Objects(_player, mapAction.button))
            {
                mapAction.button.SetActive(true);
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
            foreach (MapAction mapAction in _mapActions)
            {
                if (mapAction.hasClick && mapAction.isSingle && _actionButtonsClickDistance >= Utils.GetDistanceBetween2Objects(_player, mapAction.button))
                {
                    mapAction.button.SetActive(false);
                    mapAction.hasClick = false;
                }
            }
        }
    }
}