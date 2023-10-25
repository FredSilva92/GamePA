using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTargetScript : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;

    // Update is called once per frame
    void Update()
    {
        Ray ray =m_Camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            transform.position = raycastHit.point;
        }
    }
}
