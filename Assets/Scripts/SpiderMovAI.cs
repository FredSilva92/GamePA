using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderMovAI : MonoBehaviour
{
    public Transform destination; // O ponto de destino para onde a aranha deve ir
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetDestination(destination.position);
    }

    void SetDestination(Vector3 target)
    {
        if (navMeshAgent != null)
        {
            // Define o destino do NavMeshAgent para o ponto de destino
            navMeshAgent.SetDestination(target);
        }
    }
}

