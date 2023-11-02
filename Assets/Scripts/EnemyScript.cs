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

    private bool hasDropped = false;
    private ThirdPersonMovement playerData;

    [Header("Health Manager")]
    [SerializeField]
    private HealthManager _healthManager;

    private NavMeshAgent agent;
    private Vector3 initialPosition;

    private float timeWalking = 7f;
    private float currentTimeWalking = 0f;

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

        Debug.Log("dt: " + Time.deltaTime);

        Transform plTransform = player.transform;
        float playerDistance = Vector3.Distance(transform.position, plTransform.position);
        bool playerIsDead = playerData.IsDead;

        if (playerDistance > _minDistance && playerDistance < _maxDistance && !playerIsDead)
        {
            SetShootingAnimation(1.0f);

            animator.SetBool(Animations.WALKING, true);
            animator.SetBool(Animations.SHOOTING, true);

            _isShooting = true;
            agent.isStopped = false;
            FaceTarget();
        }
        else if (playerDistance <= _minDistance && !playerIsDead)
        {
            agent.isStopped = true;
            animator.SetBool(Animations.WALKING, false);
            animator.SetBool(Animations.SHOOTING, true);
            _isShooting = true;
            FaceTarget();
        }
        else
        {
            if (_isShooting)
            {
                agent.isStopped = false;
                agent.SetDestination(transform.position);
            }

            SetShootingAnimation(0.0f);
            StopShooting();


            animator.SetBool(Animations.WALKING, agent.remainingDistance > 0.2f);

            if (currentTimeWalking < 0)
            {
                currentTimeWalking = timeWalking;
                agent.isStopped = true;
                animator.SetBool(Animations.WALKING, false);
                RandomWalking();
            } else
            {
                currentTimeWalking -= Time.deltaTime;
            }
        }
    }

    private void RandomWalking()
    {
        destPoint = AIMovHelpers.GetDestinationPoint(initialPosition, 5f);

        agent.SetDestination(destPoint);
        transform.LookAt(destPoint);

        agent.isStopped = false;
    }

    private void SetShootingAnimation(float fadeTime)
    {
        shootWeight = Mathf.Lerp(shootWeight, fadeTime, 0.1f);
        animator.SetLayerWeight(animator.GetLayerIndex(Utils.Constants.SHOOT), shootWeight);
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

    private void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position);
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion shootRotation = Quaternion.AngleAxis(20, transform.up) * lookRotation;

        transform.rotation = Quaternion.Slerp(transform.rotation, shootRotation, Time.deltaTime * 100);

        transform.rotation = Quaternion.AngleAxis(15, transform.up) * lookRotation;
        agent.SetDestination(player.transform.position);
    }
}