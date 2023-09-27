using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private CharacterController character;
    private Animator animator;
    private Vector3 inputs;

    private float speed = 2f;
    

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inputs.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        character.Move(inputs * Time.deltaTime * speed);
        character.Move(Vector3.down * Time.deltaTime);

        if (inputs != Vector3.zero)
        {
            animator.SetBool("isWalking", true);
            transform.forward = Vector3.Slerp(transform.forward, inputs, Time.deltaTime * 10);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

  
}
