using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    enum levelList
    {
        Forest,
        Camp,
        Cave
    };

    [SerializeField]
    private levelList Level;

    [SerializeField]
    private float transitionTime = 2f;

    [SerializeField]
    private GameObject loadScreen;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            switch (gameManager.CurrentGameState.Value)
            {
                case GameState.GO_TO_FOREST:
                    gameManager.SetGameState(GameState.INTRO_FOREST);
                    Destroy(this);
                    break;
                case GameState.GO_TO_CAMP:
                    gameManager.SetGameState(GameState.INTRO_CAMP);
                    Destroy(this);
                    break;
                case GameState.GO_TO_CAVE:
                    //SceneManager.LoadScene("CaveAndPyramid");
                    gameManager.SetGameState(GameState.INTRO_CAVE);
                    //EnableScreen();
                    //StartCoroutine(LoadLevel());
                    break;
                default:
                    break;
            }
        }
    }

    private string GetLevelStr()
    {
        switch (Level)
        {
            case levelList.Forest:
                return Utils.Environments.FOREST;
            case levelList.Camp:
                return Utils.Environments.CAMP;
            case levelList.Cave:
                return Utils.SceneNames.CAVE_AND_PYRAMID;
            default:
                return "";
        }
    }

    private IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(GetLevelStr());
    }

    private void EnableScreen()
    {
        loadScreen.SetActive(true);
    }
}