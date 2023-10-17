using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonShooter : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera aimVirtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.GetButton("Fire3") ? "I'm the aim camera!" : "sd");
        aimVirtualCamera.gameObject.SetActive(Input.GetButton("Fire3"));
    }
}
