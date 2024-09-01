using UnityEngine;

public class PlayerShootingLaser : MonoBehaviour
{
    public Transform laserOrigin;    // The starting point of the laser
    public float laserRange = 100f;  // The maximum range of the laser
    public LineRenderer lineRenderer; // The LineRenderer component to draw the laser
    public LayerMask hitLayers;      // LayerMask to specify which layers the laser can hit
    public float laserDamage = 10f;  // Damage dealt by the laser

    private bool isShooting = false;
     [SerializeField] public Animator animator = null;
    [SerializeField] AudioSource laserSFX;
    [SerializeField] AudioSource laserHit;
    [SerializeField] AudioClip laserHitClip;

    private const string IS_SHOOTING = "IsShooting";
    private PlayerHealthLogic playerHealth;

    void Start()
    {
        lineRenderer.enabled = false;
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealthLogic>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !playerHealth.isHit) 
        {
            laserSFX.enabled = true;
            lineRenderer.enabled = true;  // Show the laser beam
            ShootLaser();                 // Update the laser beam position
        }
        else
        {
            lineRenderer.enabled = false; // Hide the laser beam when the button is released
            laserSFX.enabled = false;
        }
    }

    void ShootLaser()
    {
        // Set the starting point of the laser
        lineRenderer.SetPosition(0, laserOrigin.position);
        // Debug.DrawRay(laserOrigin.position, laserOrigin.forward * laserRange, Color.red);
        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, laserRange))
        {
                
            // lineRenderer.SetPosition(1, hit.point);
            lineRenderer.SetPosition(1, laserOrigin.position + laserOrigin.forward * laserRange);


            // Check if the hit object has an Enemy component
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            Level1Boss boss1 = hit.collider.GetComponent<Level1Boss>();
            Level2Boss boss2 = hit.collider.GetComponent<Level2Boss>();
            if (enemy != null)
            {
                enemy.TakeDamage(2); // Apply damage to the enemy
                if (!laserHit.isPlaying)
                {
                    laserHit.PlayOneShot(laserHitClip);
                }
            }

            if (boss1 != null)
            {
                boss1.TakeDamage(2);
                if (!laserHit.isPlaying && boss1.isVulnerable)
                {
                    laserHit.PlayOneShot(laserHitClip);
                }
            }

            if (boss2 != null)
            {
                boss2.TakeDamage(2);
                if (!laserHit.isPlaying && boss2.isVulnerable)
                {
                    laserHit.PlayOneShot(laserHitClip);
                }
            }
        }
        else
        {
            // If the raycast doesn't hit anything, set the end point of the laser to the max range
            lineRenderer.SetPosition(1, laserOrigin.position + laserOrigin.forward * laserRange);
        }
    }
}
