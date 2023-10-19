using UnityEngine;

public class EnemyScript : CharacterBase
{
    private Transform playerTransform;

    private CharacterController character;
    private Animator animator;

    [SerializeField]
    private float _speed = 2f;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private float moveRadius;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject droppableItem;

    private float _minDistance = 2f;
    private float _maxDistance = 6f;
    private float shootWeight = 0.0f;

    private Vector3 inputs;
    private Vector3 destPoint;

    private bool isMoving = false;
    private bool hasDropped = false;
    private ThirdPersonMovement playerData;

    [Header("Health Manager")]
    [SerializeField]
    private HealthManager _healthManager;

    void Start()
    {
        //playerTransform = GameObject.FindWithTag("PlayerPrefab").transform;
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerData = player.GetComponent<ThirdPersonMovement>();
    }

    void Update()
    {
        if (_isDead)
        {
            StopShooting();
            Utils.DeathAnimation(animator);
            if (!hasDropped) DropItem();
            return;
        }

        Transform plTransform = player.transform;
        float playerDistance = Vector3.Distance(transform.position, plTransform.position);
        bool playerIsDead = playerData.IsDead;

        if (playerDistance > _minDistance && playerDistance < _maxDistance && !playerIsDead)
        {

            transform.LookAt(plTransform);
            SetShootingAnimation(1.0f);

            animator.SetBool("isWalking", true);
            animator.SetBool("isShooting", true);
            _isShooting = true;
            Move();

        }
        else if (playerDistance <= _minDistance && !playerIsDead)
        {
            transform.LookAt(plTransform);
            animator.SetBool("isWalking", false);
            animator.SetBool("isShooting", true);
            _isShooting = true;
        }
        else
        {
            SetShootingAnimation(0.0f);
            StopShooting();

            if (isMoving)
            {
                if (Vector3.Distance(transform.position, destPoint) < 4f)
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

        transform.LookAt(destPoint);
        Move();
        CancelInvoke("RandomWalking");
    }

    private void SetShootingAnimation(float fadeTime)
    {
        shootWeight = Mathf.Lerp(shootWeight, fadeTime, 0.1f);
        animator.SetLayerWeight(animator.GetLayerIndex(Utils.Constants.SHOOT), shootWeight);
    }

    void OnCollisionEnter(Collision collision)
    {
        isMoving = false;
        animator.SetBool("isWalking", false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Utils.CheckIfWasHitShooted(collision, _healthManager, Utils.Constants.LAZER_BULLET_PLAYER, ref _isDead);
    }

    private void DropItem()
    {
        Instantiate(droppableItem, transform.position, Quaternion.identity);
        hasDropped = true;
    }

    private void StopShooting()
    {
        animator.SetBool("isShooting", false);
        _isShooting = false;
    }
}