using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Transform player;

    private CharacterController character;
    private Animator animator;

    [SerializeField]
    private float _speed = 2f;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private float moveRadius;

    private float _minDistance = 2f;
    private float _maxDistance = 6f;
    private float shootWeight = 0.0f;

    private Vector3 inputs;
    private Vector3 destPoint;

    private bool isMoving = false;


    void Start()
    {
        player = GameObject.FindWithTag("PlayerPrefab").transform;
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //transform.position += new Vector3(2, 0, 0);
        float playerDistance = Vector3.Distance(transform.position, player.position);
        

        if (playerDistance > _minDistance && playerDistance < _maxDistance)
        {

            transform.LookAt(player);
            SetShootingAnimation(1.0f);

            animator.SetBool("isWalking", true);
            Move();

        } else if(playerDistance <= _minDistance)
        {
            transform.LookAt(player);
            animator.SetBool("isWalking", false);
        }
        else
        {
            SetShootingAnimation(0.0f);
            if (isMoving)
            {
                if (Vector3.Distance(transform.position, destPoint) < 0.5f)
                {
                    isMoving = false;
                    animator.SetBool("isWalking", false);
                }
                else
                {
                    Move();
                }

            }
            else
            {
                Invoke("RandomWalking", 4);
            }
        }
    }

    private void Move()
    {
        animator.SetBool("isWalking", true);
        inputs.Set(transform.forward.x, 0, transform.forward.z);
        character.Move(inputs * Time.deltaTime * _speed);
        character.Move(Vector3.down * Time.deltaTime);
        transform.forward = Vector3.Slerp(transform.forward, inputs, Time.deltaTime * 10);
    }

    private void RandomWalking()
    {
        animator.SetBool("isWalking", true);
        isMoving = true;
        destPoint = AIMovHelpers.GetDestinationPoint(transform.position, 10f);

        Debug.Log("AI: " + destPoint);

        transform.LookAt(destPoint);
        Move();
        CancelInvoke("RandomWalking");
    }

    private void SetShootingAnimation(float fadeTime)
    {
        shootWeight = Mathf.Lerp(shootWeight, fadeTime, 0.1f);
        animator.SetLayerWeight(animator.GetLayerIndex("Shoot"), shootWeight);
    }

    void OnCollisionEnter(Collision collision)
    {
        isMoving = false;
        animator.SetBool("isWalking", false);
    }
}