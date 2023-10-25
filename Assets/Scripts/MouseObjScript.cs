using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MouseObjScript : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;
    // Start is called before the first frame update

    [SerializeField]
    private GameObject crossHair;

    [SerializeField]
    private GameObject aimTarget;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (aimTarget.active)
        {
            Vector3 screenCenter = new Vector3((Screen.width + Screen.width / 8) / 2f, (Screen.height - Screen.height / 8) / 2f, Camera.main.transform.position.z);
            Vector3 worldCenter = m_Camera.ScreenToWorldPoint(screenCenter);
            aimTarget.transform.position = worldCenter;
        }
        }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void AvoidPlayer(Collider other)
    {
        GameObject.FindGameObjectWithTag("Player");
    }
}
