using UnityEngine;
using UnityEngine.AI;
using static Utils;

public class EnemyScript : CharacterBase
{
    private Animator animator;

    [SerializeField]
    private float _speed = 2f;

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

    private NavMeshAgent agent;
    private Vector3 initialPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerData = player.GetComponent<ThirdPersonMovement>();
        agent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;

    }

    void Update()
    {
        if (_isDead)
        {
            StopShooting();
            PlayAnimation(animator, Animations.DYING);
            if (!hasDropped) DropItem();
            return;
        }

        Transform plTransform = player.transform;
        float playerDistance = Vector3.Distance(transform.position, plTransform.position);
        bool playerIsDead = playerData.IsDead;

        if (playerDistance > _minDistance && playerDistance < _maxDistance && !playerIsDead)
        {
            agent.SetDestination(plTransform.position);
            SetShootingAnimation(1.0f);

            animator.SetBool(Animations.WALKING, true);
            animator.SetBool(Animations.SHOOTING, true);
            _isShooting = true;

        }
        else if (playerDistance <= _minDistance && !playerIsDead)
        {
            agent.isStopped = true;
            animator.SetBool(Animations.WALKING, false);
            animator.SetBool(Animations.SHOOTING, true);
            _isShooting = true;
        }
        else
        {
            SetShootingAnimation(0.0f);
            StopShooting();

            if (!agent.hasPath)
            {
                isMoving = false;
                animator.SetBool(Animations.WALKING, false);
                Invoke("RandomWalking", 4);
            }
        }
    }

    private void RandomWalking()
    {
        animator.SetBool(Animations.WALKING, true);
        isMoving = true;
        destPoint = AIMovHelpers.GetDestinationPoint(initialPosition, 7f);

        agent.SetDestination(destPoint);
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
        animator.SetBool(Animations.WALKING, false);
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
        animator.SetBool(Animations.SHOOTING, false);
        _isShooting = false;
    }
}