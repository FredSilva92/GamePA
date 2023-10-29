using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    enum levelList
    {
        Cave,
        Forest
    };

    [SerializeField]
    private levelList Level;

    [SerializeField]
    private float transitionTime = 3f;

    [SerializeField]
    private GameObject loadScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            loadScreen.SetActive(true);
            StartCoroutine(LoadLevel());
        }
    }

    private string GetLevelStr()
    {
        switch (Level)
        {
            case levelList.Cave:
                return Utils.Environments.CAVE;
            case levelList.Forest:
                return Utils.Environments.FOREST;
            default:
                return "";
        }
    }

    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene("Scenes/" + GetLevelStr());
    }
}