using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    public float speed = 5.0f;
    public float maxDistance = 10.0f;
    private Vector3 firstPosition;
    private bool walkingForward = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        firstPosition = transform.position;
    }

    void Update()
    {
        if (walkingForward)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            animator.SetBool("isWalking", true);

            if (Vector3.Distance(firstPosition, transform.position) >= maxDistance)
            {
                walkingForward = false;
                animator.SetBool("isWalking", true);
                transform.Rotate(Vector3.up, 180f);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, firstPosition, speed * Time.deltaTime);

            if (Vector3.Distance(firstPosition, transform.position) < 0.1f)
            {
                walkingForward = true;
                animator.SetBool("isWalking", true);
            }
        }
    }
}