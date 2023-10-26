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

    /*
     * Esconde todos os bot�es de a��o no mapa por padr�o.
     * Observa o estado atual do jogo, para que sempre que haja uma mudan�a o evento seja acionado.
    */
    private void Start()
    {
        HideAllActionButtons();

        // assina o observ�vel para detetar mudan�as de estado
        _currentGameState.Subscribe(state =>
        {
            HandleGameStateChange(state);
        });
    }

    private void Update()
    {
        // n�o h� necessidade de verificar se clicou no bot�o de a��o,
        // quando o estado � apenas de mostrar uma cutscene
        if (_currentGameState.Value != GameState.INTRO_GAME ||
            _currentGameState.Value != GameState.INTRO_FOREST ||
            _currentGameState.Value != GameState.INTRO_CAVE)
        {
            CheckActionButtonsVisibilityDistance();
            CheckActionButtonsClickDistance();
        }
    }

    /*
     * Trata da mudan�a para os diferentes estados do jogo.
     * _currentMapActions[0] - porque existe sempre pelo menos uma a��o do mapa associada a um game state
    */
    private void HandleGameStateChange(GameState nextGameState)
    {
        /* CONFIGURA��ES PADR�O NA MUDAN�A DE ESTADO */

        HideCurrentActionButtons();

        _currentGameState.Value = nextGameState;
        _currentMapActions = GetCurrentMapActions();

        if (_currentMapActions[0].gameStateInfo.hasNewPosition)
        {
            ChangePlayerPosition(_currentMapActions[0].gameStateInfo.position, _currentMapActions[0].gameStateInfo.rotation);
        }

        /* CONFIGURA��ES ESPEC�FICAS NA MUDAN�A DE ESTADO */

        switch (nextGameState)
        {
            case GameState.INTRO_FOREST:
                Debug.Log("O estado do jogo mudou para INTRO_FOREST");

                OnCutsceneStart(_currentMapActions[0].gameStateInfo.cutscene);

                nextGameState = GetNextGameState(_currentGameState.Value);

                // evento de t�rmino da cutscene
                _currentMapActions[0].gameStateInfo.cutscene.loopPointReached += (videoPlayer) => OnCutsceneEnd(_currentMapActions[0].gameStateInfo.cutscene, nextGameState);
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

            if (_currentGameState.Value == GameState.GO_TO_FOREST)
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
}