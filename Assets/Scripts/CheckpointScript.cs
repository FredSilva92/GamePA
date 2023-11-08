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
        gameManagerInstance.LastCheckPointPos = transform.position;
    }
}
