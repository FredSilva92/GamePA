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


    /* MÉTODOS */

    /*
     * Permite apenas uma instância de GameManager por cena.
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

            // evento de término da cutscene
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
     * Procura a ação atual que diz respeito ao objetivo, que será a que tem "hasProgress" como true,
     * e devolve o título.
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
     * Altera o estado do jogo e atualiza a lista de ações do mapa atuais.
    */
    private void TransitionGameState(GameState nextGameState)
    {
        HideCurrentActionButtons();

        _currentGameState = nextGameState;
        _currentMapActions = GetCurrentMapActions();
    }

    /*
     * Ao iniciar a cena, todos os botões de ação são ocultos por padrão.
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
     * Esconde os botões das ações do estado atual.
     * Utilizável quando o utilizador transita para o próximo estado de jogo e as ações do estado anterior já não interessam.
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
     * Se o jogador estiver perto das ações de jogo, o botão de ação será visível, caso contrário continuará oculto.
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
     * Se o jogador estiver muito perto das ações de jogo, o botão de ação poderá ser clicado, caso contrário não.
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