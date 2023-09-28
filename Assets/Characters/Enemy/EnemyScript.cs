using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    private CharacterController character;
    private Animator animator;
    private Vector3 inputs;

    private float speed;

    private int a = 0;

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
        character.Move((transform.forward * inputs.magnitude * Time.deltaTime * speed));
        character.Move((Vector3.down * Time.deltaTime));

        if (inputs != Vector3.zero)
        {
            animator.SetBool("isWalking", true);
            transform.forward = Vector3.Slerp(transform.forward, inputs, Time.deltaTime * 10);

            Debug.Log("Forward: " + transform.forward);
            Debug.Log("Conttar: " + a++);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (Input.GetButton("Fire1"))
        {
            animator.SetBool("isShooting", true);
        }
        else
        {
            animator.SetBool("isShooting", false);
        }

    }
}
