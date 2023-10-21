using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {

    }

    public void PlayGame()
    {
        gameManager.CurrentState = GameState.InIntro;
        SceneManager.LoadScene("Pause_Cutscenes_Goals");  // SceneManager.LoadScene("IslandPart1");
    }
}