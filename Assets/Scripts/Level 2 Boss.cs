using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Level2Boss : MonoBehaviour
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
    [SerializeField] float damageCooldown = 0.5f;

    private float introTimer = 0f;
    public bool isRoaring = true;
    private bool isDead = false;
    public bool isIdle = false;
    public bool isSpellCasting = false;
    public bool isVulnerable = false;

    [SerializeField] int health = 30;
    [SerializeField] int damagePerHit = 1;
    [SerializeField] GameObject vulnerableMesh;

    [SerializeField] GameObject finishPoint;
    [SerializeField] AudioSource playAudio;
    [SerializeField] AudioSource levelClearAudio;
    [SerializeField] ParticleSystem bossDamageEffect;
    [SerializeField] ParticleSystem bossDeathEffect;
    [SerializeField] ParticleSystem bossSpell;

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
        GameManager.Instance.ShowBossHealth(health); // Show boss health when boss appears
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
        if (introTimer > 3)
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

        if (isSpellCasting)
        {
            bossSpell.Play();
        }
        else if (!isSpellCasting)
        {
            bossSpell.Stop();
        }
    }

    private void RunTowardsPlayer()
    {
        if (!isRoaring && !isIdle && !isDead)
        {
            if (bossAgent.isActiveAndEnabled && !isSpellCasting)
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
        int attackChoice = Random.Range(1, 3); // Randomly choose between 1 and 2

        if (!isAttacked && isPlayerInAttackRange)
        {

            if (attackChoice % 2 == 0)
            {
                bossSFX.PlayOneShot(bossAttackingClip);
                animator.SetTrigger("Attack1"); // Trigger the flying kick attack animation
            }
            else
            {
                bossSFX.PlayOneShot(bossAttackingClip);
                animator.SetTrigger("Attack2"); // Trigger the hurricane kick & spell cast animation
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
        GameManager.Instance.UpdateBossHealth(health); // Update GameManager with new boss health
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
        finishPoint.SetActive(true);
        levelClearAudio.Play();

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // Handle damage to the player here
            playerHealth.TakeDamage(3);
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
