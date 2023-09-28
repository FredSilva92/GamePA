using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController character;
    private Animator animator;

    public Transform camera;

    public float speed = 2f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;


    void Start()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horinzontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horinzontal, 0f, vertical).normalized;
        bool isWalking = direction.magnitude > 0;

        if (isWalking)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            character.Move(moveDirection * speed * Time.deltaTime);

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}