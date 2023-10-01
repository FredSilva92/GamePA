using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private Vector3 inputs;

    private float shootWeight = 0.0f;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        inputs.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool isWalking = inputs != Vector3.zero;

        if (isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        float fadeTime = Input.GetButton("Fire1") ? 1.0f : 0.0f;

        shootWeight = Mathf.Lerp(shootWeight, fadeTime, 0.05f);
        animator.SetLayerWeight(animator.GetLayerIndex("Shoot"), shootWeight);
    }
}