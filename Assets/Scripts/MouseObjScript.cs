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
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
        int playerLayer = LayerMask.NameToLayer("Player"); // Assuming your player is on a layer named "Player"
        int layerMask = ~(1 << playerLayer);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 50f, layerMask))
        {
            if (aimTarget.active) aimTarget.transform.position = raycastHit.point;
            
        }

        if (crossHair.active) {
            Vector3 mousePosition = Input.mousePosition;

            Transform croosHairTransform = crossHair.GetComponent<RectTransform>().transform;
            croosHairTransform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
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
