using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    private Debugging_PlayerHealth playerHealth;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask groundMask;
    public float health;

    // enemy patrolling
    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] float walkPointRange;

    // enemy attacking
    [SerializeField] float detectionRange;
    [SerializeField] float attackRange;
    private bool isPlayerInDetectionRange;
    private bool isPlayerInAttackRange;
    private bool isAttacked;

    [SerializeField] float attackCooldown;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletOrigin; // Assign your rifle nozzle in the Inspector
    [SerializeField] float bulletSpeed; // Adjust the speed of the bullets
    [SerializeField] AudioSource audioSource;
    [SerializeField] int damagePerHit;

    [SerializeField] ParticleSystemRenderer healthIndicator;
    [SerializeField] Material threeHitMat;
    [SerializeField] Material twoHitMat;
    [SerializeField] Material oneHitMat;
    [SerializeField] AudioSource enemyDead;
    [SerializeField] ParticleSystem deathParticles;

    private bool isShooting = false;
    private const string IS_SHOOTING = "IsShooting";
    [SerializeField] private Animator animator = null;

    private void Awake()
    {
        player = GameObject.Find("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player GameObject not found. Make sure it is named 'Player'.");
            return;
        }

        playerHealth = player.GetComponent<Debugging_PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("Debugging_PlayerHealth script not found on Player GameObject.");
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on Enemy GameObject.");
            return;
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on Enemy GameObject.");
            }
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null)
        {
            // If player or playerHealth is null, return early
            return;
        }

        isPlayerInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerMask);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (!isPlayerInDetectionRange && !isPlayerInAttackRange)
        {
            EnemyPatrol();
        }
        if (isPlayerInDetectionRange && playerHealth.GetHealth() > 0)
        {
            ChasePlayer();
        }
        if (isPlayerInAttackRange && playerHealth.GetHealth() > 0)
        {
            AttackPlayer();
        }

        if (health == 3)
        {
            healthIndicator.material = threeHitMat;
        }
        else if (health == 2)
        {
            healthIndicator.material = twoHitMat;
        }
        else
        {
            healthIndicator.material = oneHitMat;
        }
    }

    private void EnemyPatrol()
    {
        if (!walkPointSet)
        {
            SearchForWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkPoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchForWalkPoint()
    {
        float randPointZ = Random.Range(-walkPointRange, walkPointRange);
        float randPointX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randPointX, transform.position.y, transform.position.z + randPointZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.position);
        }
    }

    void AttackPlayer()
    {
        //stop chasing & look at player
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        isShooting = true;
        animator.SetBool(IS_SHOOTING, isShooting);

        if (!isAttacked && isPlayerInAttackRange)
        {
            // Instantiate the bullet at the nozzle's position and rotation
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.SetPositionAndRotation(bulletOrigin.position, bulletOrigin.rotation);

            // Get the Rigidbody component of the bullet
            // Set the bullet's velocity to move in the direction the player is facing then destroy it after 2 seconds
            audioSource.Play();
            bullet.GetComponent<Rigidbody>().velocity = bulletOrigin.forward * bulletSpeed;
            Destroy(bullet, 2);

            isAttacked = true;
            Invoke("ResetAttack", attackCooldown);
        }
    }

    private void ResetAttack()
    {
        isAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        // Implement damage logic
        health -= damage;

        if (health <= 0)
        {
            deathParticles.Play();
            enemyDead.Play();
            Invoke("DestroyEnemy", 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        if (GameManager.Instance != null)
        {
            Debug.Log("Updating GameManager with enemy kill");
            GameManager.Instance.AddEnemyKill(); // Notify GameManager of enemy death
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }
    }

    // indicators for enemy AI detection range and attack range (for level design)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            TakeDamage(damagePerHit);
            Destroy(collision.gameObject);
        }
    }
}
