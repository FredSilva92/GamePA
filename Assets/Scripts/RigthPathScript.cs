using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigthPathScript : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject wall;

    private ThirdPersonMovement ThirdPersonMovement;

    // Start is called before the first frame update
    void Start()
    {
        ThirdPersonMovement = player.GetComponent<ThirdPersonMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (wall.active)
        {
            return;
        }

        if(Utils.Environments.FOREST.Equals(ThirdPersonMovement.CurrentEnvironment))
        {
            wall.SetActive(true);
        }*/
    }
}
