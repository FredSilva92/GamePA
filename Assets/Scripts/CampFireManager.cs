using UnityEngine;

public class CampFireController : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform player;

    public float maxVolume = 0.45f;
    public float minVolume = 0.01f;
    public float proximityRadius = 20f;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = Camera.main.transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        float volume = Mathf.Lerp(minVolume, maxVolume, 1 - (distance / proximityRadius));
        volume = Mathf.Clamp01(volume);

        audioSource.volume = volume;

        if (distance <= proximityRadius)
        {
            // se est� dentro do range
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // se est� fora do range
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}