using UnityEngine;

public class PlayerShootingAreaDamage : MonoBehaviour
{
    public float shootingInterval = 1f;   // Time interval between shots

    private bool isShooting = false;
    private float lastShootTime;

    void Update()
    {
        // Check if the fire button is pressed and if enough time has passed
        if (Input.GetButton("Fire1") && Time.time >= lastShootTime + shootingInterval)
        {
            isShooting = true;            
            lastShootTime = Time.time; // Update the last shoot time
        } else {
            isShooting = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected!!");
        EnemyAI enemy = collision.collider.GetComponent<EnemyAI>();
        Level1Boss boss1 = collision.collider.GetComponent<Level1Boss>();
        Level2Boss boss2 = collision.collider.GetComponent<Level2Boss>();
        if (enemy != null)
        {
            enemy.TakeDamage(3); // Apply damage to the enemy
        }

        if (boss1 != null)
        {
            boss1.TakeDamage(3);
        }

        if (boss2 != null)
        {
            boss2.TakeDamage(3);
        }
    }
}
