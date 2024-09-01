using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Level2Boss : MonoBehaviour
{
    // Variables for player position & Boss movement
    private Transform player;
    [SerializeField] LayerMask playerMask;
    [SerializeField] NavMeshAgent bossAgent = null;
    [SerializeField] Animator animator = null;
    private Rigidbody rb;

    // Variables for boss attacks
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
        if (animator == null) animator = GetComponentInChildren<Animator>();

        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealthLogic>();

        bossSFX.PlayOneShot(bossIntroDeathClip);
        GameManager.Instance.ResetBossHealth(); // Reset boss health when it first appears
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
        else
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

            int attackChoice = Random.Range(1, 3); // Randomly choose between 1 and 2

            if (!isAttacked && isPlayerInAttackRange)
            {
                bossSFX.PlayOneShot(bossAttackingClip);
                animator.SetTrigger(attackChoice == 1 ? "Attack1" : "Attack2");

                isAttacked = true;
                Invoke("ResetAttack", attackCooldown);
            }
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
            isDead = true;
            BossDeathSequence();
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
        bossSFX.PlayOneShot(bossIntroDeathClip);
        bossAgent.enabled = false;
        animator.Play("Boss Dying");
        playAudio.Stop();
        bossDeathEffect.Play();
        Invoke("HandleBossDefeat", 7f); // Handle defeat after boss death sequence
    }

    private void HandleBossDefeat()
    {
        Destroy(gameObject);
        GameManager.Instance.HideBossHealth(); // Hide boss health when boss is destroyed
        finishPoint.SetActive(true);
        levelClearAudio.Play();

        // Trigger the transition screen after the boss is defeated
        GameManager.Instance.ShowTransitionScreen(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
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