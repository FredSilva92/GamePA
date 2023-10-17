using System.Collections;
using UnityEngine;

public class ThirdPersonMovement : CharacterBase
{
    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed;
    public float airMinSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    private bool isItemToPick;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState currentState;

    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        air
    }

    public bool freeze;
    public bool unlimited;

    public bool restricted;

    bool keepMomentum;

    private Animator animator;

    [Header("Health Bar")]
    [SerializeField]
    private GameObject healthBar;

    private HealthManager _healthManager;

    public HealthManager HealthManager
    {
        get { return _healthManager; }
        private set { _healthManager = value; }
    }



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        _healthManager = healthBar.GetComponent<HealthManager>();
        HealthManager = _healthManager;

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isDead)
        {
            Debug.Log("I'm Death");
            return;
        };
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (currentState == MovementState.walking)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            Debug.Log("I'm Death");
            return;
        };
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        _isShooting = Input.GetButton(Utils.Constants.SHOOT_KEY);
    }

    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            currentState = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        // Mode - Unlimited
        else if (unlimited)
        {
            currentState = MovementState.unlimited;
            desiredMoveSpeed = 999f;
        }

        // Mode - Walking
        else if (grounded)
        {
            currentState = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            currentState = MovementState.air;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        // deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (restricted) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // on ground
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        // in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    private void OnTriggerEnter(Collider collision)
    {
      
        Utils.CheckIfWasHitShooted(collision, _healthManager, Utils.Constants.LAZER_BULLET_ENEMY, ref _isDead);

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Medicine") && Input.GetButton(Utils.Constants.PICK))
        {
            GameObject gameObject = collision.gameObject;
            MedicineScript medicineScript = gameObject.GetComponent<MedicineScript>();
            _healthManager.UpdateHealth(medicineScript.Health);

            Destroy(collision.gameObject);
        }
    }


}