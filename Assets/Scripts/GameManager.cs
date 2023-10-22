using UnityEngine;

public enum GameState
{
    MainMenu,   // quando está no menu
    InIntro,    // quando está a iniciar as cena de jogo
    InGame      // depois da cutscene inicial
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] private GameState _currentState;

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

    public GameState CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }

    /*
     * Permite apenas uma instância de GameManager por cena.
    */
    void Awake()
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

    void Start()
    {
        // dados das ações do jogador no mapa
        MapAction[] actions = new MapAction[]
        {
            new MapAction(1, "Intro to the game", 1, true, true, false, false),
            new MapAction(2, "Hide ship", 2, true, true, true, false),
            new MapAction(3, "Wrong path 1 to the forest", 3, false, false, true, true),
            new MapAction(4, "Wrong path 2 to the forest", 3, false, false, true, true),
            new MapAction(5, "Correct path to the forest", 3, true, false, true, false)
        };
    }

    void Update()
    {
        if (CurrentState == GameState.InIntro)
        {

        }
    }
}