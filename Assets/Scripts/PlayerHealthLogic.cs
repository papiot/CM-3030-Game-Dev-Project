using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerHealthLogic : MonoBehaviour
{
    [SerializeField] public int health = 50; // Set initial health
    [SerializeField] public int lives = 3;
    [SerializeField] private float respawnDelay = 2f; // Delay before the player respawns
    public bool isDead = false; // flag for player death
    private int initialHealth;
    private bool particlesCollided = false; // flag for paricle collision
    [SerializeField] private float collisionCooldown = 2f;
    [SerializeField] private Animator animator = null;
    [SerializeField] private ParticleSystem playerInjured;
    private AudioSource playerSFX;
    [SerializeField] private AudioClip playerHitAudio;

    void Start()
    {
        initialHealth = health;
        lives = GameManager.Instance.GetLivesLeft(); // Get lives from GameManager at the start
        GameManager.Instance.SetPlayerHealth(health);
        playerSFX = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponent<Animator>();
        //Debug.Log("Player Health initialized: " + health);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy damage"))
        {
            TakeDamage(1); // Assuming fixed damage value of 1
            Destroy(collision.gameObject);

            if (GameManager.Instance != null)
            {
                Debug.Log("Updating GameManager with new health: " + health);
                if (health >= 0)
                {
                    GameManager.Instance.SetPlayerHealth(health); // Notify GameManager of the new health value
                }
                else if (health < 0)
                {
                    GameManager.Instance.SetPlayerHealth(0); // Notify GameManager of the new health value
                }
            }
            else
            {
                Debug.LogError("GameManager instance is null.");
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!particlesCollided)
        {
            TakeDamage(5);

            if (GameManager.Instance != null)
            {
                Debug.Log("Particles Hit - new health: " + health);
                if (health >= 0)
                {
                    GameManager.Instance.SetPlayerHealth(health); // Notify GameManager of the new health value
                }
                else if (health < 0)
                {
                    GameManager.Instance.SetPlayerHealth(0); // Notify GameManager of the new health value
                }
            }
            particlesCollided = true;
            Invoke("ResetCollisionCooldown", collisionCooldown);
        }
    }

    private void ResetCollisionCooldown()
    {
        particlesCollided = false;
    }


    public void TakeDamage(int damage)
    {
        if (!isDead && health > 0)
        {
            health -= damage;
            Debug.Log("Player Health: " + health);
            animator.SetTrigger("IsHit");
            if (!playerSFX.isPlaying)
            {
                playerSFX.PlayOneShot(playerHitAudio);
            }
            playerInjured.Play();
            

            if (health <= 0)
            {
                PlayerDeathSequence();
            }
        }
    }

    private void PlayerDeathSequence()
    {
        if (!isDead)
        {
            isDead = true;
            gameObject.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
            animator.Play("Death");
            lives--;

            Debug.Log("Player Died. Lives remaining: " + lives);
            GameManager.Instance.SetLivesLeft(lives); // Update lives in GameManager

            if (lives > 0)
            {
                StartCoroutine(RestartLevel());
            }
            else
            {
                GameOver();
            }
        }
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Reset player state
        health = initialHealth;
        isDead = false;
        GameManager.Instance.SetPlayerHealth(health);

        // Respawn player / reload the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //private void GameOver()
    //{
    //    Debug.Log("Game Over. No lives left.");
    //    // Implement your Game Over logic here, such as loading a Game Over screen
    //    SceneManager.LoadSceneAsync(0);
    //    GameManager.Instance.SetLivesLeft(3);
    //}

    private void GameOver()
    {
        Debug.Log("Game Over. No lives left.");
        GameManager.Instance.ShowTransitionScreen(true); // Show transition screen when player dies with no lives left
    }
}