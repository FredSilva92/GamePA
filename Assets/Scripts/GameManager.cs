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

    /*
     * Esconde todos os botões de ação no mapa.
     * Obtém as ações atuais do estado de jogo inicial, pré-definido.
     * Inicia a cutscene inicial.
    */
    private void Start()
    {
        HideAllActionButtons();

        _currentMapActions = GetCurrentMapActions();

        // quando o estado pré-definido é apenas para mostrar a cutscene
        // aciona o evento de progredir, que irá iniciar a cutscene de imediato
        if (_currentGameState == GameState.INTRO_GAME ||
            _currentGameState == GameState.INTRO_FOREST ||
            _currentGameState == GameState.INTRO_CAVE)
        {
            // [0] - porque existe sempre pelo menos uma ação ligada a um game state
            ProgressActionEvent(_currentMapActions[0]);
        }
    }

    private void Update()
    {
        if (_currentGameState != GameState.INTRO_GAME ||
            _currentGameState != GameState.INTRO_FOREST ||
            _currentGameState != GameState.INTRO_CAVE)
        {
            CheckActionButtonsVisibilityDistance();
            CheckActionButtonsClickDistance();
        }
        else  // se for um dos estados que é apenas a cutscene, progride de imediato
        {
            //ProgressActionEvent(_currentMapActions[0]);
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

    /*
     * Obtém o próximo estado do jogo pelo seu número identificador.
    */
    private GameState GetNextGameState(int gameStateNumber)
    {
        int nextGameStateNumber = gameStateNumber + 1;
        return (GameState)nextGameStateNumber;
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

        if (_currentMapActions[0].gameStateInfo.hasNewPosition)
        {
            _player.transform.localPosition = _currentMapActions[0].gameStateInfo.position;
            _player.transform.localRotation = Quaternion.Euler(_currentMapActions[0].gameStateInfo.rotation);
        }
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
            if (mapAction.hasClick)
            {
                if (_actionButtonsVisibilityDistance >= Utils.GetDistanceBetween2Objects(_player, mapAction.button))
                {
                    mapAction.button.SetActive(true);
                }
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
                if (mapAction.hasClick)
                {
                    if (_actionButtonsClickDistance >= Utils.GetDistanceBetween2Objects(_player, mapAction.button))
                    {
                        ProgressActionEvent(mapAction);
                        NoProgressActionEvent(mapAction);
                    }
                }
            }
        }
    }

    /*
     * Quando ação clicada permite que o jogador progrida no jogo.
     * Quando o jogador alcança um objetivo, este não pode voltar ao objetivo anterior.
     * Exemplo: quando o encontra o esconderijo para a nave.
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

            if (mapAction.gameStateInfo.hasCutscene)
            {
                OnCutsceneStart(mapAction.gameStateInfo.cutscene);

                GameState nextGameState = GetNextGameState((int)_currentGameState);

                // evento de término da cutscene
                mapAction.gameStateInfo.cutscene.loopPointReached += (videoPlayer) => OnCutsceneEnd(mapAction.gameStateInfo.cutscene, nextGameState);
            }
        }
    }

    /*
     * Quando ação clicada não permite que o jogador progrida no jogo.
     * Exemplo: quando o jogador tenta um caminho errado para a floresta.
    */
    private void NoProgressActionEvent(MapAction mapAction)
    {
        if (!mapAction.hasProgress)
        {
            if (mapAction.hasDialogue)
            {
                Debug.Log("Ação sem progressão! Halley deve falar algo!");
            }
        }
    }
}