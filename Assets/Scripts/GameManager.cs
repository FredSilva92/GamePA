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

    }

    void Update()
    {
        if (CurrentState == GameState.InIntro)
        {

        }
    }
}