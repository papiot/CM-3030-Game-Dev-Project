using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject weapon3;
    public GameObject weapon4;

    public int currentWeapon = 1;

    private GameObject nozzleParent;
    private Transform currWeaponTransform;
    private GameObject activeWeapon;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask; // Ensure this includes your Floor layer

    private float jumpForce = 7f;
    private bool isJumping = false;

    private float horInput;
    private float verInput;
    [SerializeField] private float moveForce = 3f;

    private Rigidbody myRigidBody;
    private int gameScore;

    private bool isWalking = false;
    private const string IS_WALKING = "IsWalking";
    [SerializeField] private Animator animator = null;

    [Header("Dashing Settings")]
    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private bool useCameraForward = true;
//    [SerializeField] private bool allowAllDirections = true;
    [SerializeField] private bool disableGravity = false;
    [SerializeField] private bool resetVel = true;
    [SerializeField] private float dashCd = 2f;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    private bool isDashing = false;
    private float dashCdTimer;
    private Vector3 delayedForceToApply;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
        gameScore = 0;

        // Set the groundMask to include the Floor layer
        groundMask = LayerMask.GetMask("Floor");

        // Find the "nozzle" GameObject by name
        nozzleParent = GameObject.Find(GameObject.Find("player_nozzle").transform.parent.gameObject.name);

        GameObject currWeapon = GameObject.Find("player_nozzle");
        currWeaponTransform = currWeapon.transform;

        Debug.Log("Nozzle Parent: " + nozzleParent);

        Debug.Log("Parent Name: " + GameObject.Find("player_nozzle").transform.parent.gameObject.name);
    }

    void Update()
    {
        HandleInput();
        RotatePlayerToMouseCursor();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        HandleJump();
    }

    private void HandleInput()
    {
        horInput = Input.GetAxis("Horizontal");
        verInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            isJumping = true;
        }

        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }

        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }

        // Handle weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(4);
        }
    }

    void SwitchWeapon(int weaponNum) 
    {
        Debug.Log("Switching weapon to weapon " + weaponNum);
        GameObject currWeapon = GameObject.Find("player_nozzle");
        if (currWeapon != null) 
        {
            currWeapon.SetActive(false);
        }

        if (activeWeapon) {
            activeWeapon.SetActive(false);
        }
        
        if (weaponNum == 1) {
            activeWeapon = Instantiate(weapon1);
            PlayerShooting playerShooting = activeWeapon.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
               playerShooting.animator = animator; 
            }
            activeWeapon.transform.SetParent(nozzleParent.transform); 
            activeWeapon.transform.position = currWeaponTransform.position;
            activeWeapon.transform.rotation = currWeaponTransform.rotation;
            activeWeapon.SetActive(true);
        }
        if (weaponNum == 2) {
            activeWeapon = Instantiate(weapon2);
            PlayerShooting playerShooting = activeWeapon.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
               playerShooting.animator = animator; 
            }
            activeWeapon.transform.SetParent(nozzleParent.transform);
            activeWeapon.transform.position = currWeaponTransform.position;
            activeWeapon.transform.rotation = currWeaponTransform.rotation;
            activeWeapon.SetActive(true);
        }
        if (weaponNum == 3) {
            activeWeapon = Instantiate(weapon3);
            PlayerShooting playerShooting = activeWeapon.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
               playerShooting.animator = animator; 
            }
            activeWeapon.transform.SetParent(nozzleParent.transform);
            activeWeapon.transform.position = currWeaponTransform.position;
            activeWeapon.transform.rotation = currWeaponTransform.rotation;
            activeWeapon.SetActive(true);
        }
        if (weaponNum == 4) {
            activeWeapon = Instantiate(weapon4); 
            PlayerShooting playerShooting = activeWeapon.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
               playerShooting.animator = animator; 
            }
            activeWeapon.transform.SetParent(nozzleParent.transform);
            activeWeapon.transform.position = currWeaponTransform.position;
            activeWeapon.transform.rotation = currWeaponTransform.rotation;
            activeWeapon.SetActive(true);
        }
        
        if (currentWeapon == 1) {
            // Destroy(weapon1);
            weapon1.SetActive(false);
        }
        if (currentWeapon == 2) {
            weapon2.SetActive(false);
        }
        if (currentWeapon == 3) {
            weapon3.SetActive(false);
        }
        if (currentWeapon == 4) {
            weapon4.SetActive(false);
        }

        currentWeapon = weaponNum;

        
    }
    private void MovePlayer()
    {
        if (!isDashing)
        {
            Vector3 moveDir = new Vector3(horInput * moveForce, myRigidBody.velocity.y, verInput * moveForce);
            myRigidBody.velocity = moveDir;
            isWalking = horInput != 0 || verInput != 0;
            animator.SetBool(IS_WALKING, isWalking);
        }
    }

    private void RotatePlayerToMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Keep the player upright
            transform.forward = direction; // Rotate player to face the direction
        }
    }

    private void HandleJump()
    {
        if (isJumping)
        {
            isWalking = false;
            myRigidBody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isJumping = false;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        bool grounded = Physics.Raycast(groundCheck.position, Vector3.down, out hit, 0.2f, groundMask);

        // Debug log to see if the raycast is hitting something
        //if (grounded)
        //{
        //    Debug.Log("Grounded on: " + hit.collider.gameObject.name);
        //}
        //else
        //{
        //    Debug.Log("Not grounded");
        //}

        // Draw the raycast in the scene view for visual debugging
        Debug.DrawRay(groundCheck.position, Vector3.down * 0.2f, grounded ? Color.green : Color.red);

        return grounded;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            gameScore += 1;
            GetComponent<AudioSource>().Play();
            Destroy(other.gameObject);
            Debug.Log("Game Score = " + gameScore);
        }
    }

    private void Dash()
    {
        if (dashCdTimer > 0 || !IsGrounded()) return; // Add IsGrounded check here
        else dashCdTimer = dashCd;

        isDashing = true;
        myRigidBody.velocity = Vector3.zero;

        Transform forwardT = useCameraForward ? Camera.main.transform : transform;
        Vector3 direction = GetDirection(forwardT);

        delayedForceToApply = direction * dashForce;

        if (disableGravity)
            myRigidBody.useGravity = false;

        StartCoroutine(DelayedDashForce());
        StartCoroutine(ResetDash());
    }

    private IEnumerator DelayedDashForce()
    {
        yield return new WaitForSeconds(0.025f);
        if (resetVel)
            myRigidBody.velocity = Vector3.zero;

        myRigidBody.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private IEnumerator ResetDash()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        if (disableGravity)
            myRigidBody.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);

        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }

        return direction;
    }
}
