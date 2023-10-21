using UnityEngine;
using static Utils;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField]
    private GameObject playerMainComp;

    private Animator animator;
    private Vector3 inputs;

    private float shootWeight = 0.0f;

    private ThirdPersonMovement thridPersonMovement;

    private int layerShootIdx;
    private bool _isShooting;

    public bool IsShooting
    {
        get { return _isShooting; }
    }


    void Start()
    {
        thridPersonMovement = playerMainComp.GetComponent<ThirdPersonMovement>();
        animator = GetComponent<Animator>();
        layerShootIdx = animator.GetLayerIndex(Constants.SHOOT);
    }

    void Update()
    {
        // ---------- MORRER ----------
        if (thridPersonMovement.IsDead)
        {
            PlayAnimation(animator, Animations.DYING);
            return;
        } else if (thridPersonMovement.IsPicking)
        {
            PlayAnimation(animator, Animations.PICKING);
            return;
        }

        animator.SetBool(Animations.PICKING, false);
        // ---------- SALTAR ----------

        // se termina de saltar
        if (thridPersonMovement.FinishedJump)
        {
            animator.SetBool(Animations.JUMPING, false);
        }
        else if (!thridPersonMovement.FinishedJump && thridPersonMovement.currentState == ThirdPersonMovement.MovementState.air)
        {
            animator.SetBool(Animations.JUMPING, true);
        }


        // ---------- ANDAR ----------

        inputs.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool isWalking = inputs != Vector3.zero;

        animator.SetBool(Animations.WALKING, isWalking);
        

        bool isAiming = Input.GetButton(Constants.AIM_KEY) || Input.GetButton(Constants.SHOOT_KEY);

        float fadeTime = isAiming ? 1.0f : 0.0f;


        shootWeight = Mathf.Lerp(shootWeight, fadeTime, 0.5f);
        animator.SetLayerWeight(layerShootIdx, shootWeight);

        animator.SetBool(Animations.SHOOTING, thridPersonMovement.IsShooting);
    }
}