using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Level1Boss : MonoBehaviour
{
    // variables for player posisiton & Boss movement
    private Transform player; // Reference to the player
    [SerializeField] LayerMask playerMask;
    [SerializeField] float speed = 12f; // Speed at which the boss runs
    [SerializeField] float jumpForce = 10f; // Force applied for the jump attack
    [SerializeField] NavMeshAgent bossAgent = null;
    [SerializeField] Animator animator = null; // Reference to the Animator component
    private Rigidbody rb; // Reference to the Rigidbody component

    //variables for boss attacks
    // enemy attaking
    [SerializeField] float detectionRange = 20f;
    [SerializeField] float attackRange = 3f;
    private bool isPlayerInDetectionRange;
    private bool isPlayerInAttackRange;
    private bool isAttacked;
    [SerializeField] float attackCooldown = 2f;


    private bool isJumping = false; // Flag to indicate if the boss is currently jumping
    private bool isPunching = false; // Flag to indicate if the boss is currently punching
    private bool isClawSwiping = false; // Flag to indicate if the boss is currently doing the claw swipe attack

    [SerializeField] int health = 30;
    [SerializeField] int damagePerHit = 1;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody>();
        if (bossAgent == null) bossAgent = GetComponent<NavMeshAgent>();
        if(animator == null) animator = GetComponentInChildren<Animator>();
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
    }

    private void RunTowardsPlayer()
    {
        if (bossAgent.isActiveAndEnabled && !isAttacked)
        {
            bossAgent.SetDestination(player.position);
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
            Invoke("ResetAttack", attackCooldown);
        }
    }

    private void ResetAttack()
    {
        isAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Invoke("DestroyEnemy", 1f);
        }
    }


    private void DestroyEnemy()
    {
        Destroy(gameObject);

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            TakeDamage(damagePerHit);
            Destroy(collision.gameObject);
        }

        if (isJumping && collision.gameObject.CompareTag("Player"))
        {
            // Handle damage to the player here
            Debug.Log("Boss hit the player with jump attack!");
        }

        if (isPunching && collision.gameObject.CompareTag("Player"))
        {
            // Handle damage to the player here
            Debug.Log("Boss hit the player with punch attack!");
        }

        if (isClawSwiping && collision.gameObject.CompareTag("Player"))
        {
            // Handle damage to the player here
            Debug.Log("Boss hit the player with claw swipe attack!");
        }
    }



    // This method is called by an animation event at the start of the jump attack animation
    public void StartJumpAttack()
    {
        isJumping = true;
        Vector3 direction = (player.position - transform.position).normalized;
        rb.AddForce(new Vector3(direction.x, 1, direction.z) * jumpForce, ForceMode.VelocityChange);
    }

    // This method is called by an animation event at the end of the jump attack animation
    public void EndJumpAttack()
    {
        isJumping = false;
    }

    // This method is called by an animation event at the start of the punching animation
    public void StartPunching()
    {
        isPunching = true;
    }

    // This method is called by an animation event at the end of the punching animation
    public void EndPunching()
    {
        isPunching = false;
        // Automatically transition to Claw Swipe after Punching
        animator.SetTrigger("Attack3"); // Trigger the claw swipe animation
    }

    // This method is called by an animation event at the start of the claw swipe animation
    public void StartClawSwipe()
    {
        isClawSwiping = true;
    }

    // This method is called by an animation event at the end of the claw swipe animation
    public void EndClawSwipe()
    {
        isClawSwiping = false;
    }
}
