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

    /*
     * Esconde todos os bot�es de a��o no mapa.
     * Obt�m as a��es atuais do estado de jogo inicial, pr�-definido.
     * Inicia a cutscene inicial.
    */
    private void Start()
    {
        HideAllActionButtons();

        _currentMapActions = GetCurrentMapActions();

        // quando o estado pr�-definido � apenas para mostrar a cutscene
        // aciona o evento de progredir, que ir� iniciar a cutscene de imediato
        if (_currentGameState == GameState.INTRO_GAME ||
            _currentGameState == GameState.INTRO_FOREST ||
            _currentGameState == GameState.INTRO_CAVE)
        {
            // [0] - porque existe sempre pelo menos uma a��o ligada a um game state
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
        else  // se for um dos estados que � apenas a cutscene, progride de imediato
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

    /*
     * Obt�m o pr�ximo estado do jogo pelo seu n�mero identificador.
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
     * Altera o estado do jogo e atualiza a lista de a��es do mapa atuais.
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
     * Quando a��o clicada permite que o jogador progrida no jogo.
     * Quando o jogador alcan�a um objetivo, este n�o pode voltar ao objetivo anterior.
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

                // evento de t�rmino da cutscene
                mapAction.gameStateInfo.cutscene.loopPointReached += (videoPlayer) => OnCutsceneEnd(mapAction.gameStateInfo.cutscene, nextGameState);
            }
        }
    }

    /*
     * Quando a��o clicada n�o permite que o jogador progrida no jogo.
     * Exemplo: quando o jogador tenta um caminho errado para a floresta.
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