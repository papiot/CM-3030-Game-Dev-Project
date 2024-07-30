using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Level1Boss : MonoBehaviour
{
    // variables for player posisiton & Boss movement
    private Transform player; // Reference to the player
    [SerializeField] LayerMask playerMask;
    [SerializeField] NavMeshAgent bossAgent = null;
    [SerializeField] Animator animator = null; // Reference to the Animator component
    private Rigidbody rb; // Reference to the Rigidbody component

    //variables for boss attacks
    [SerializeField] float detectionRange = 20f;
    [SerializeField] float attackRange = 3f;
    private bool isPlayerInDetectionRange;
    private bool isPlayerInAttackRange;
    private bool isAttacked;
    [SerializeField] float attackCooldown = 4f;
    private bool bossVulnerable = false;

    private float introTimer = 0f;
    private bool isRoaring = true;

    [SerializeField] int health = 30;
    [SerializeField] int damagePerHit = 1;
    [SerializeField] GameObject vulnerableMesh;

    [SerializeField] GameObject finishPoint;
    [SerializeField] AudioSource playAudio;
    [SerializeField] AudioSource levelClearAudio;

    private Debugging_PlayerHealth playerHealth;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody>();
        if (bossAgent == null) bossAgent = GetComponent<NavMeshAgent>();
        if(animator == null) animator = GetComponentInChildren<Animator>();

        playerHealth = GameObject.Find("Player").GetComponent<Debugging_PlayerHealth>();
    }

    void Update()
    {
        isPlayerInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerMask);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        if (isPlayerInDetectionRange)
        {
            RunTowardsPlayer();
        }
        if (isPlayerInAttackRange)
        {
            CheckForAttack();
        }

        introTimer += Time.deltaTime;
        if(introTimer >= 4f)
        {
            isRoaring = false;
            animator.SetBool("Is_Roaring", false);
        }

        if (bossVulnerable)
        {
            vulnerableMesh.SetActive(true);
        }
        else if (!bossVulnerable)
        {
            vulnerableMesh.SetActive(false);
        }
    }

    private void RunTowardsPlayer()
    {
        if (!isRoaring && !isAttacked)
        {
            if (bossAgent.isActiveAndEnabled && !isAttacked)
            {
                bossAgent.SetDestination(player.position);
            }
        }
    }

    private void CheckForAttack()
    {
        bossAgent.SetDestination(transform.position);
        transform.LookAt(player);
        // Randomly choose between Jump Attack and Punch/Swipe Attack
        int attackChoice = Random.Range(0, 10); // Randomly choose between 0 and 9

        if (!isAttacked && isPlayerInAttackRange)
        {

            if (attackChoice < 5)
            {
                animator.SetTrigger("Attack1"); // Trigger the jump attack animation
            }
            else
            {
                animator.SetTrigger("Attack2"); // Trigger the punching animation
            }

            isAttacked = true;
            bossVulnerable = true;
            Invoke("ResetAttack", attackCooldown);
        }
    }

    private void ResetAttack()
    {
        bossVulnerable = false;
        isAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        if (bossVulnerable)
        {
            health -= damage;
            Debug.Log("Boss Hit");
        }

        if(health <= 0)
        {
            animator.SetBool("Is_Dead", true);
            bossAgent.isStopped = true;
            playAudio.Stop();
            levelClearAudio.Play();
            finishPoint.SetActive(true);
            //Invoke("DestroyEnemy", 10f);
        }
    }


    private void DestroyEnemy()
    {
        Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // Handle damage to the player here
            playerHealth.health -= 2;
            Debug.Log("Player Hit. Player Health: " + playerHealth.health);
        }
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
