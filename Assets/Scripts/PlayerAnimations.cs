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
    private bool _isShooting;

    public bool IsShooting { 
        get { return _isShooting;} 
    }


    void Start()
    {
        thridPersonMovement = playerMainComp.GetComponent<ThirdPersonMovement>();
        animator = GetComponent<Animator>();
        layerShootIdx = animator.GetLayerIndex(Utils.Constants.SHOOT);
    }

    void Update()
    {
        if (thridPersonMovement.IsDead)
        {
            Utils.DeathAnimation(animator);
            return;
        }

        inputs.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool isWalking = inputs != Vector3.zero;

        animator.SetBool("isWalking", isWalking);

        
        bool isAiming = Input.GetButton(Utils.Constants.AIM_KEY) || Input.GetButton(Utils.Constants.SHOOT_KEY);

        float fadeTime = isAiming ? 1.0f : 0.0f;
        

        shootWeight = Mathf.Lerp(shootWeight, fadeTime, 0.5f);
        animator.SetLayerWeight(layerShootIdx, shootWeight);

        //Debug.Log("Shoot weight: " + shootWeight);

        //_isShooting = Input.GetButton(Utils.Constants.SHOOT_KEY);
        animator.SetBool("isShooting", thridPersonMovement.IsShooting);
        
    }
}