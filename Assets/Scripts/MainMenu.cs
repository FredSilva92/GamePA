using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private GameObject painelMainMenu;
    [SerializeField] private GameObject painelControls;
    [SerializeField] private GameObject painelCredits;

    // Start is called before the first frame update
    public void Start()
    {
        SceneManager.LoadScene(levelName);
    }

    // Update is called once per frame
    public void OpenControls()
    {
        painelMainMenu.SetActive(false);
        painelControls.SetActive(true);
    }

    public void CloseControls()
    {
        painelControls.SetActive(false);
        painelMainMenu.SetActive(true);
    }

    public void OpenCredits()
    {
        painelMainMenu.SetActive(false);
        painelCredits.SetActive(true);
    }
    public void CloseCredits()
    {
        painelCredits.SetActive(false);
        painelMainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("sair do jogo");
        Application.Quit();
    }
}
