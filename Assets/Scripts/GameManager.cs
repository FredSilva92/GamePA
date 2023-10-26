using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UniRx;

public class GameManager : MonoBehaviour
{
    /* ATRIBUTOS */

    private static GameManager _instance;

    [SerializeField] private GameObject _player;

    [SerializeField] private ReactiveProperty<GameState> _currentGameState = new();

    [SerializeField] private List<GameStateInfo> _gameStateList;
    [SerializeField] private List<MapAction> _mapActions;

    private List<MapAction> _currentMapActions = new();

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
     * Garante apenas uma instância de GameManager por cena.
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
     * Esconde todos os botões de ação no mapa por padrão.
     * Observa o estado atual do jogo, para que sempre que haja uma mudança o evento seja acionado.
    */
    private void Start()
    {
        HideAllActionButtons();

        // assina o observável para detetar mudanças de estado
        _currentGameState.Subscribe(state =>
        {
            HandleGameStateChange(state);
        });
    }

    private void Update()
    {
        // não há necessidade de verificar se clicou no botão de ação,
        // quando o estado é apenas de mostrar uma cutscene
        if (_currentGameState.Value != GameState.INTRO_GAME ||
            _currentGameState.Value != GameState.INTRO_FOREST ||
            _currentGameState.Value != GameState.INTRO_CAVE)
        {
            CheckActionButtonsVisibilityDistance();
            CheckActionButtonsClickDistance();
        }
    }

    /*
     * Trata da mudança para os diferentes estados do jogo.
     * _currentMapActions[0] - porque existe sempre pelo menos uma ação do mapa associada a um game state
    */
    private void HandleGameStateChange(GameState nextGameState)
    {
        /* CONFIGURAÇÕES PADRÃO NA MUDANÇA DE ESTADO */

        HideCurrentActionButtons();

        _currentGameState.Value = nextGameState;
        _currentMapActions = GetCurrentMapActions();

        if (_currentMapActions[0].gameStateInfo.hasNewPosition)
        {
            ChangePlayerPosition(_currentMapActions[0].gameStateInfo.position, _currentMapActions[0].gameStateInfo.rotation);
        }

        /* CONFIGURAÇÕES ESPECÍFICAS NA MUDANÇA DE ESTADO */

        switch (nextGameState)
        {
            case GameState.INTRO_FOREST:
                Debug.Log("O estado do jogo mudou para INTRO_FOREST");

                OnCutsceneStart(_currentMapActions[0].gameStateInfo.cutscene);

                nextGameState = GetNextGameState(_currentGameState.Value);

                // evento de término da cutscene
                _currentMapActions[0].gameStateInfo.cutscene.loopPointReached += (videoPlayer) => OnCutsceneEnd(_currentMapActions[0].gameStateInfo.cutscene, nextGameState);
                break;
            default:
                break;
        }
    }

    /*
     * Obtém as ações atuais do mapa associadas ao estado de jogo atual.
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
     * Procura a ação atual que diz respeito ao objetivo,
     * que será a que tem "hasProgress" como true e devolve o título.
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
        ChangeGameState(nextGameState);
    }

    private void ChangePlayerPosition(Vector3 position, Vector3 rotation)
    {
        _player.transform.localPosition = position;
        _player.transform.localRotation = Quaternion.Euler(rotation);
    }

    public void ChangeGameState(GameState newGameState)
    {
        _currentGameState.Value = newGameState;
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
            if (mapAction.hasClick)
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
        foreach (MapAction mapAction in _currentMapActions)
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
     * Quando ação clicada permite que o jogador progrida no jogo (passe para o próximo game state).
     * Quando o jogador alcança um objetivo, este não pode voltar ao objetivo anterior.
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

            if (_currentGameState.Value == GameState.GO_TO_FOREST)
            {
                GameState nextGameState = GetNextGameState(_currentGameState.Value);
                ChangeGameState(nextGameState);
            }

            if (mapAction.gameStateInfo.hasCutscene)
            {
                OnCutsceneStart(mapAction.gameStateInfo.cutscene);

                GameState nextGameState = GetNextGameState(_currentGameState.Value);

                // evento de término da cutscene
                mapAction.gameStateInfo.cutscene.loopPointReached += (videoPlayer) => OnCutsceneEnd(mapAction.gameStateInfo.cutscene, nextGameState);
            }
        }
    }

    /*
     * Quando ação clicada não permite que o jogador progrida no jogo (passe para o próximo game state).
     * Exemplo: Quando o jogador tenta um caminho errado para a floresta. Quando observa uma pista na floresta.
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