using UnityEngine;

public class RigthPathScript : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject wall;

    private ThirdPersonMovement ThirdPersonMovement;

    void Start()
    {
        ThirdPersonMovement = player.GetComponent<ThirdPersonMovement>();
    }

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