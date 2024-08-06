using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Debugging_PlayerHealth : MonoBehaviour
{
    [SerializeField] public int health = 20; // Set initial health
    [SerializeField] private Transform respawnPoint; // The point where the player will respawn
    [SerializeField] private float respawnDelay = 3f; // Delay before the player respawns

    private int initialHealth;

    void Start()
    {
        initialHealth = health;
        Debug.Log("Player Health initialized: " + health);
    }

    void Update()
    {
        // No update logic needed for hit registration
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy damage"))
        {
            TakeDamage(10); // Assuming fixed damage value of 10
            Destroy(collision.gameObject);

            if (GameManager.Instance != null)
            {
                Debug.Log("Updating GameManager with new health: " + health);
                GameManager.Instance.SetPlayerHealth(health); // Notify GameManager of the new health value
            }
            else
            {
                Debug.LogError("GameManager instance is null.");
            }
        }
    }

    public int GetHealth()
    {
        return health;
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");
        // Start the respawn process
        StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(respawnDelay);

        // Restart the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
