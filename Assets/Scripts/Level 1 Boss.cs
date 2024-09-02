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
    private bool isHit;
    [SerializeField] float attackCooldown = 3f;
    [SerializeField] float damageCooldown = 0.25f;

    private float introTimer = 0f;
    private bool isRoaring = true;
    private bool isDead = false;
    public bool isIdle = false;
    public bool isVulnerable = false;

    [SerializeField] int health = 30;
    [SerializeField] int damagePerHit = 1;
    [SerializeField] GameObject vulnerableMesh;

    [SerializeField] GameObject finishPoint;
    [SerializeField] AudioSource playAudio;
    [SerializeField] AudioSource levelClearAudio;
    [SerializeField] ParticleSystem bossDamageEffect;
    [SerializeField] ParticleSystem bossDeathEffect;

    [SerializeField] AudioSource bossSFX;
    [SerializeField] AudioClip bossIntroDeathClip;
    [SerializeField] AudioClip bossHitClip;
    [SerializeField] AudioClip bossAttackingClip;

    private PlayerHealthLogic playerHealth;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody>();
        if (bossAgent == null) bossAgent = GetComponent<NavMeshAgent>();
        if(animator == null) animator = GetComponentInChildren<Animator>();

        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealthLogic>();

        bossSFX.PlayOneShot(bossIntroDeathClip);
        GameManager.Instance.ResetBossHealth();  // Reset boss health when it first appears
        GameManager.Instance.ShowBossHealth(health);  // Show boss health when boss appears
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

        if (isVulnerable)
        {
            vulnerableMesh.SetActive(true);
        }
        else if (!isVulnerable)
        {
            vulnerableMesh.SetActive(false);
        }
    }

    private void RunTowardsPlayer()
    {
        if (!isRoaring && !isAttacked && !isIdle && !isDead)
        {
            if (bossAgent.isActiveAndEnabled && !isAttacked)
            {
                bossAgent.SetDestination(player.position);
            }
        }
    }

    private void CheckForAttack()
    {
        if (!isDead)
        {
            bossAgent.SetDestination(transform.position);
            transform.LookAt(player);
        }
        // Randomly choose between Jump Attack and Punch/Swipe Attack
        int attackChoice = Random.Range(0, 10); // Randomly choose between 0 and 9

        if (!isAttacked && isPlayerInAttackRange)
        {

            if (attackChoice < 5)
            {
                bossSFX.PlayOneShot(bossAttackingClip);
                animator.SetTrigger("Attack1"); // Trigger the jump attack animation
            }
            else
            {
                bossSFX.PlayOneShot(bossAttackingClip);
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

    private void ResetDamage()
    {
        isHit = false;
    }

    public void TakeDamage(int damage)
    {
        if (isVulnerable && !isHit)
        {
            isHit = true;
            BossDamageSequence(damage);
            Invoke("ResetDamage", damageCooldown);
        }

        if (health <= 0)
        {
            if (!isDead)
            {
                //isDead = true;
                BossDeathSequence();
            }
        }
    }

    private void BossDamageSequence(int damage)
    {
        bossDamageEffect.Play();
        health -= damage;
        // Update GameManager with new boss health
        if (health >= 0)
        {
            GameManager.Instance.UpdateBossHealth(health);
        }
        else if(health < 0)
        {
            GameManager.Instance.UpdateBossHealth(0);
        }
        if (!bossSFX.isPlaying)
        {
            bossSFX.PlayOneShot(bossHitClip);
        }
        Debug.Log("Boss Hit. Boss Health: " + health);
    }

    private void BossDeathSequence()
    {
        isDead = true;
        bossSFX.PlayOneShot(bossIntroDeathClip);
        bossAgent.enabled = false;
        animator.Play("Boss Dying");
        playAudio.Stop();
        bossDeathEffect.Play();
        Invoke("DestroyEnemy", 7f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        GameManager.Instance.HideBossHealth();  // Hide boss health when boss is destroyed

        finishPoint.SetActive(true);
        levelClearAudio.Play();

        GameManager.Instance.ShowTransitionScreen(false); // Show transition screen when the boss dies
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // Handle damage to the player here
            playerHealth.TakeDamage(2);
            if (playerHealth.health >= 0)
            {
                GameManager.Instance.SetPlayerHealth(playerHealth.health); // Notify GameManager of the new health value
            }
            else if (playerHealth.health < 0)
            {
                GameManager.Instance.SetPlayerHealth(0); // Notify GameManager of the new health value
            }
            Debug.Log("Player Hit. Player Health: " + playerHealth.health);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            Destroy(collision.gameObject);
            TakeDamage(damagePerHit);
        }
    }

}
