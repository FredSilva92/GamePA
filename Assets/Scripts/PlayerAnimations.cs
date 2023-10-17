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
        // ---------- MORRER ----------
        if (thridPersonMovement.IsDead)
        {
            Utils.DeathAnimation(animator);
            return;
        }


        // ---------- SALTAR ----------

        // se termina de saltar
        if (thridPersonMovement.FinishedJump)
        {
            animator.SetBool("isJumping", false);
        }
        else if (!thridPersonMovement.FinishedJump && thridPersonMovement.currentState == ThirdPersonMovement.MovementState.air)
        {
            animator.SetBool("isJumping", true);
        }


        // ---------- ANDAR ----------

        inputs.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool isWalking = inputs != Vector3.zero;

        animator.SetBool("isWalking", isWalking);


        // ---------- DISPARAR ----------

        float fadeTime = Input.GetButton("Fire1") ? 1.0f : 0.0f;

        shootWeight = Mathf.Lerp(shootWeight, fadeTime, 0.1f);
        animator.SetLayerWeight(layerShootIdx, shootWeight);
    }
}