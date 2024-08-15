using UnityEngine;

public class PlayerShootingBalistic : MonoBehaviour
{
    public GameObject bulletPrefab;       // The bullet prefab
    public Transform bulletOrigin;        // The origin point for shooting bullets
    public float bulletForce = 20f;       // The forward force applied to the bullet
    public float verticalForce = 15f;     // The upward force applied to the bullet
    public float shootingInterval = 1f;   // Time interval between shots

    private float lastShootTime;

    void Update()
    {
        // Check if the fire button is pressed and if enough time has passed
        if (Input.GetButton("Fire1") && Time.time >= lastShootTime + shootingInterval)
        {
            Shoot();
            lastShootTime = Time.time; // Update the last shoot time
        }
    }

    void Shoot()
    {
        // Instantiate the bullet prefab at the bullet origin's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, bulletOrigin.position, bulletOrigin.rotation);

        // Get the Rigidbody component of the bullet
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

        // Calculate the force vector with upward and forward components
        Vector3 force = bulletOrigin.forward * bulletForce + bulletOrigin.up * verticalForce;

        // Apply the calculated force to the bullet
        bulletRigidbody.AddForce(force, ForceMode.Impulse);
    }
}
