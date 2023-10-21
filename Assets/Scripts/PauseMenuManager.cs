using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    private bool isPaused = false;

    void Start()
    {
        // oculta menu de pausa ao abrir a cena
        pauseMenuPanel.SetActive(false);
    }

    /*
     * Se pressiona "ESC", abre o menu de pausa;
     * Com o menu de pausa ativo, se pressiona "ESC" ou clica no bot�o de continuar, retorna ao jogo.
    */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // desbloqueia este atributo, uma vez que � bloqueado quando a c�mera de jogo est� ativa
            Cursor.lockState = CursorLockMode.None;

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // desbloqueia este atributo, uma vez que � bloqueado quando a c�mera de jogo est� ativa
        Cursor.lockState = CursorLockMode.None;

        // congela o tempo
        Time.timeScale = 0;

        isPaused = true;
        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        // volta ao estado normal da c�mera de jogo
        Cursor.lockState = CursorLockMode.Locked;

        // descongela o tempo
        Time.timeScale = 1;

        isPaused = false;
        pauseMenuPanel.SetActive(false);
    }

    public void QuitGame()
    {
        // descongela o tempo
        Time.timeScale = 1;

        SceneManager.LoadScene("MainMenu");
    }
}