using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField]
    private GameObject playerMainComp;

    private Animator animator;
    private Vector3 inputs;

    private float shootWeight = 0.0f;

    private ThirdPersonMovement thridPersonMovement;

    private int layerShootIdx;

    void Start()
    {
        thridPersonMovement = playerMainComp.GetComponent<ThirdPersonMovement>();
        animator = GetComponent<Animator>();
        layerShootIdx = animator.GetLayerIndex(Utils.Constants.SHOOT);
    }

    void Update()
    {

        if (thridPersonMovement.HealthManager.Health <= 0)
        {
            isDead();
            return;
        }

        inputs.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool isWalking = inputs != Vector3.zero;

        animator.SetBool("isWalking", isWalking);

        float fadeTime = Input.GetButton("Fire1") ? 1.0f : 0.0f;
        

        shootWeight = Mathf.Lerp(shootWeight, fadeTime, 0.1f);
        animator.SetLayerWeight(layerShootIdx, shootWeight);
    }

    private void isDead()
    {
        animator.SetBool("isWalking", false);
        animator.SetLayerWeight(layerShootIdx, Mathf.Lerp(shootWeight, 0.0f, 0.2f));
        animator.SetBool("isDying", true);
    }
}