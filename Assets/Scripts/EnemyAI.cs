using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask groundMask;
    public float health;

    // enemy patroling
    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] float walkPointRange;

    // enemy attaking
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


    // [SerializeField] ParticleSystemRenderer healthIndicator;
    [SerializeField] Material threeHitMat;
    [SerializeField] Material twoHitMat;
    [SerializeField] Material oneHitMat;
    [SerializeField] AudioSource enemyDead;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] MeshRenderer healthIndicator;

    private bool isShooting = false;
    private const string IS_SHOOTING = "IsShooting";
    [SerializeField] private Animator animator = null;
    float agentSpeed;



    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agentSpeed = agent.speed;

        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        isPlayerInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerMask);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        //Debug.Log("Detection Range: " + isPlayerInDetectionRange);  // for debuggig, comment out before build
        // Debug.Log("Attack Range: " + isPlayerInAttackRange);       // for debugging comment out before build


        if (isPlayerInAttackRange)
        {
            AttackPlayer();
        }
        else if (isPlayerInDetectionRange)
        {
            ChasePlayer();
        }
        else
        {
            EnemyPatrol();
        }




        if(health == 3)
        {
            healthIndicator.material = threeHitMat;
        }
        else if(health == 2)
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
        // animation controller booleans
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsShooting", false);

        agent.speed = agentSpeed;

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
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchForWalkPoint()
    {
        float randPointZ = Random.Range(-walkPointRange, walkPointRange);
        float randPointX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randPointX, transform.position.y, transform.position.z + randPointZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
        {
            walkPointSet = true;
        }

    }

    private void ChasePlayer()
    {
        if (agent.isActiveAndEnabled)
        {
            // animation controller booleans
            animator.SetBool("IsChasing", true);
            animator.SetBool("IsShooting", false);

            agent.speed = agentSpeed * 2.5f;
            agent.SetDestination(player.position);
        }
    }


    void AttackPlayer()
    {
        // animation controller booleans
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsShooting", true);

        //stop chasing & look at player
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        isShooting = true;
        animator.SetBool(IS_SHOOTING, isShooting);

        if (!isAttacked && isPlayerInAttackRange)
        {
            //for enemies that use Projectile Missiles --> MOVE TO DIFFERENT SCRIPT?
            ///Attack code here
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            // Instantiate the bullet at the nozzle's position and rotation
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.SetPositionAndRotation(bulletOrigin.position, bulletOrigin.rotation);

            // Get the Rigidbody component of the bullet
            // Set the bullet's velocity to move in the direction the player is facing then destory it after 5 seconds
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

        if(health <= 0)
        {
            deathParticles.Play();
            if (!enemyDead.isPlaying)
            {
                enemyDead.Play();
            }
            Invoke("DestroyEnemy", 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        GameManager.Instance.AddEnemyKill(); // Notify GameManager of enemy death
        Destroy(gameObject);
        
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
        if (collision.gameObject.tag == "bullet")
        {
            TakeDamage(damagePerHit);
            Destroy(collision.gameObject);
        }
    }
}
