using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform player;

    public float maxVolume = 0.45f;
    public float minVolume = 0.01f;
    public float proximityRadius = 20f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        float volume = Mathf.Lerp(minVolume, maxVolume, 1 - (distance / proximityRadius));
        volume = Mathf.Clamp01(volume);

        audioSource.volume = volume;

        if (distance <= proximityRadius)
        {
            // se está dentro do range
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // se está fora do range
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}

