using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

public class EnemyScript : MonoBehaviour
{

    private Transform player;

    private CharacterController character;
    private Animator animator;

    [SerializeField]
    private float _speed = 2f;

    [SerializeField]
    private float _rotationSpeed;

    private float _minDistance = 2f;

    private Player _playerMovement;
    private Vector3 inputs;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindWithTag("Player").transform;


        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        _playerMovement = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log("DIst: " + Vector3.Distance(transform.position, player.position));

        transform.LookAt(player);

        if (Vector3.Distance(transform.position, player.position) > _minDistance)
        {
            inputs.Set(transform.position.x, 0, transform.position.z);
            animator.SetBool("isWalking", true);
            

            character.Move((transform.forward * inputs.magnitude * Time.deltaTime * _speed));
            character.Move((Vector3.down * Time.deltaTime));
        } else
        {
            animator.SetBool("isWalking", false);
        }


        //agent.SetDestination(player.transform.position);
        /*
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

        int shootLayerIdx = animator.GetLayerIndex("Shoot");

        if (Input.GetButton("Fire1"))
        {
            animator.SetLayerWeight(shootLayerIdx, 1);
        }
        else
        {
            animator.SetLayerWeight(shootLayerIdx, 0);
        }*/

    }

    private void FixedUpdate()
    {
        UpdatetargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdatetargetDirection()
    {

    }

    private void RotateTowardsTarget()
    {

    }

    private void SetVelocity()
    {

    }
}
