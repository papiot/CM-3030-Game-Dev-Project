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


    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Update()
    {
        isPlayerInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerMask);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        //Debug.Log("Detection Range: " + isPlayerInDetectionRange);  // for debuggig, comment out before build
        // Debug.Log("Attack Range: " + isPlayerInAttackRange);       // for debugging comment out before build

        if (!isPlayerInDetectionRange && !isPlayerInAttackRange)
        {
            EnemyPatrol();
        }
        if(isPlayerInDetectionRange)
        {
            ChasePlayer();
        }
        if(isPlayerInAttackRange)
        {
            AttackPlayer();
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
            agent.SetDestination(player.position);
        }
    }


    void AttackPlayer()
    {
        //stop chasing & look at player
        agent.SetDestination(transform.position);
        transform.LookAt(player);

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
            Destroy(bullet, 5);

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
            Invoke("DestroyEnemy", 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        
    }

    // alerts when enemy AI is in detection range and / or attack range
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
        }
    }
}
