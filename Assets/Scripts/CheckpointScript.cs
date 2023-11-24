using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    private GameManager gameManagerInstance;

    void Start()
    {
        gameManagerInstance = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Utils.Constants.PLAYER_TAG)) return;
        
        gameManagerInstance.LastCheckPointPos = transform.position;
    }
}
