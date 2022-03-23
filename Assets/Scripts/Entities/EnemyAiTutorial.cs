
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    enum MonsterState
    {
        Sleep,              // not activated
        Stand,              // standing still
        Patrol,             // patrolling the area
        Sight,              // spotted the player
        Chase,              // chasing the player
        Leap,               // jumping attack
        LongRangeAttack,    // attack from long range (shooting, etc)
        CloseRangeAttack,   // attack from close range (melee, smash, etc)
        Reload,             // reloading
        Hurt,               // taking damage
        Dead                // dead
    }

    [SerializeField]
    MonsterData m_data;

    MonsterState m_state;
    AudioSource m_audioSource;
    
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    MDLAnimator m_animator;

    [SerializeField]
    public MDLAnimation shotAnimation;

    //States
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("First Person Conroller").transform;
        agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<MDLAnimator>();
    }

    private void Update()
    {
        playerInSightRange = Vector3.Distance(transform.position, player.transform.position) < m_data.ai.sightDistance? true : false;
        playerInAttackRange = Vector3.Distance(transform.position, player.transform.position) < m_data.ai.combatRange? true : false;
        //Check for sight and attack range
        //playerInSightRange = Physics.CheckSphere(transform.position, m_data.ai.sightDistance, whatIsPlayer);
        //playerInAttackRange = Physics.CheckSphere(transform.position, m_data.ai.combatRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!m_animator.isAnimationPlaying) {
        PlayRandomAnimation(m_data.animations.patrol);
        }

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (!m_animator.isAnimationPlaying) {
            PlayRandomAnimation(m_data.animations.chase);
            PlayRandomAudioClip(m_data.audio.sight);
        }

        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked && health > 0)
        { 
            PlayRandomAnimation(m_data.animations.attackLongRange);
            PlayRandomAudioClip(m_data.audio.attack);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health > 0) {
        PlayRandomAnimation(m_data.animations.pain);
        PlayRandomAudioClip(m_data.audio.pain);
        } else {
            PlayRandomAnimation(m_data.animations.death);
            PlayRandomAudioClip(m_data.audio.death);
            Invoke(nameof(DestroyEnemy), 1f);
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    void PlayRandomAnimation(MDLAnimation[] animations)
    {
        if (animations != null && animations.Length > 0)
        {
            var index = Random.Range(0, animations.Length);
            m_animator.PlayAnimation(animations[index]);
        }
    }

    protected void PlayRandomAudioClip(AudioClip[] clips)
    {
        if (clips == null)
        {
            Debug.LogWarning("Can't play random sound: audio clips are null");
            return;
        }

        if (clips.Length == 0)
        {
            Debug.LogWarning("Can't play random sound: clips are empty");
            return;
        }

        if (m_audioSource == null)
        {
            m_audioSource = GetComponent<AudioSource>();
            if (m_audioSource == null)
            {
                Debug.LogError("Can't play random sound: AudioSource component is missing");
                return;
            }
        }

        m_audioSource.clip = clips[Random.Range(0, clips.Length)];
        m_audioSource.Play();
    }
}
