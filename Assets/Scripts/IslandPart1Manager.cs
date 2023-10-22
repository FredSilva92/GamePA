using UnityEngine;
using UnityEngine.Video;

public class IslandPart1Manager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private VideoPlayer initialCutscene;
    [SerializeField] private Canvas canvas;

    void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager.CurrentState == GameState.InIntro)
        {
            canvas.enabled = false;

            OnCutscenePlay();

            // evento de término da cutscene inicial
            initialCutscene.loopPointReached += OnCutsceneEnd;
        }
    }

    void Update()
    {

    }

    void OnCutscenePlay()
    {
        Time.timeScale = 0f;
        initialCutscene.enabled = true;
        initialCutscene.Play();
    }

    void OnCutsceneEnd(VideoPlayer vp)
    {
        initialCutscene.enabled = false;
        Time.timeScale = 1f;
    }
}