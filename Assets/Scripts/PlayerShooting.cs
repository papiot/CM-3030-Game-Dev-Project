using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    [SerializeField] GameObject bulletPrefab; // Assign your bullet prefab in the Inspector
    [SerializeField] Transform bulletOrigin; // Assign your rifle nozzle in the Inspector
    [SerializeField] float bulletSpeed = 20f; // Adjust the speed of the bullets

    [SerializeField] int numBulletMulti = 1; // the number of bullets for multi-shooting. 1 is for regular shooting

    public float spreadAngle = 5f;

    [SerializeField] AudioSource audioSource;

    [SerializeField] public Animator animator = null;
    private bool isShooting = false;

    private const string IS_SHOOTING = "IsShooting";
    private PlayerHealthLogic playerHealth;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealthLogic>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("r")) // Fire1 is typically the left mouse button
        {
            if (!playerHealth.isHit)
            {
                isShooting = true;
                Shoot();
            }
        }   
        animator.SetBool(IS_SHOOTING, isShooting);
    }

    void Shoot()
    {
        isShooting = false;

        for (int i = 0; i < numBulletMulti; i++) {
            // Instantiate the bullet at the nozzle's position and rotation
            GameObject bullet = Instantiate(bulletPrefab);
            
            // Get the Rigidbody component of the bullet
            // Set the bullet's velocity to move in the direction the player is facing
            bullet.transform.SetPositionAndRotation(bulletOrigin.position, bulletOrigin.rotation);

            // Apply random spread to the bullet's rotation
            float randomSpread = Random.Range(-spreadAngle, spreadAngle);
            Quaternion spreadRotation = Quaternion.Euler(0, randomSpread, 0);
            bullet.transform.rotation = bullet.transform.rotation * spreadRotation;

            bullet.GetComponent<Rigidbody>().velocity = bulletOrigin.forward * bulletSpeed;

            Destroy(bullet, 2);
        }
        
        audioSource.Play();
        //bullet.GetComponent<Rigidbody>().velocity = bulletOrigin.forward * bulletSpeed;

        
    }
}

//for FINDING A SCRIPT TO CALL A SPECIFIC FUNCTION!
//stateHandler = GameObject.FindGameObjectWithTag("Timer").GetComponent<StateHandler>();