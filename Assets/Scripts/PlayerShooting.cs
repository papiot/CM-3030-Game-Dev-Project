using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    [SerializeField] GameObject bulletPrefab; // Assign your bullet prefab in the Inspector
    [SerializeField] Transform bulletOrigin; // Assign your rifle nozzle in the Inspector
    [SerializeField] float bulletSpeed = 20f; // Adjust the speed of the bullets

    [SerializeField] AudioSource audioSource;

    [SerializeField] Animator animator = null;
    private bool isShooting = false;

    private const string IS_SHOOTING = "IsShooting";

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("r")) // Fire1 is typically the left mouse button
        {
            isShooting = true;
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Pressing ONE");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Pressing TWO");
        }
        animator.SetBool(IS_SHOOTING, isShooting);
    }

    void Shoot()
    {
        isShooting = false;
        // Instantiate the bullet at the nozzle's position and rotation
        GameObject bullet = Instantiate(bulletPrefab);

        bullet.transform.SetPositionAndRotation(bulletOrigin.position, bulletOrigin.rotation);

        // Get the Rigidbody component of the bullet
        // Set the bullet's velocity to move in the direction the player is facing
        audioSource.Play();
        bullet.GetComponent<Rigidbody>().velocity = bulletOrigin.forward * bulletSpeed;

        Destroy(bullet, 2);
    }
}

//for FINDING A SCRIPT TO CALL A SPECIFIC FUNCTION!
//stateHandler = GameObject.FindGameObjectWithTag("Timer").GetComponent<StateHandler>();