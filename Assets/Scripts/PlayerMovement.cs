using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask; // Ensure this includes your Floor layer

    private float jumpForce = 7f;
    private bool isJumping = false;
    private const string IS_JUMPING = "IsJumping";

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
    [SerializeField] private KeyCode dashKey = KeyCode.Mouse1;
    private AudioSource playerSFX;
    [SerializeField] private AudioClip coinCollectedAudio;

    private bool isDashing = false;
    private float dashCdTimer;
    private Vector3 delayedForceToApply;

    private PlayerHealthLogic playerHealth;

    private PauseMenu gameState;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        playerHealth = GetComponent<PlayerHealthLogic>();
        playerSFX = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponent<Animator>();
        gameScore = 0;

        // Set the groundMask to include the Floor layer
        groundMask = LayerMask.GetMask("Floor");

        gameState = GameObject.Find("PersistentManager").GetComponent<PauseMenu>();
       
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

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !playerHealth.isDead && !gameState.isPaused)
        {
            isJumping = true;
            animator.SetBool(IS_JUMPING, isJumping);
        }

        if (Input.GetKeyDown(dashKey) && !playerHealth.isDead && !gameState.isPaused)
        {
            Dash();
        }

        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }
        
    }
    private void MovePlayer()
    {
        if (!isDashing && !playerHealth.isDead && !gameState.isPaused)
        {
            Vector3 moveDir = new Vector3(horInput * moveForce, myRigidBody.velocity.y, verInput * moveForce);
            myRigidBody.velocity = moveDir;
            bool isMoving = horInput != 0 || verInput != 0;

            if (isMoving && IsGrounded())
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            animator.SetBool(IS_WALKING, isWalking);
        }
    }

    private void RotatePlayerToMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            if (!playerHealth.isDead)
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = (targetPosition - transform.position).normalized;
                direction.y = 0; // Keep the player upright
                transform.forward = direction; // Rotate player to face the direction
            }
        }
    }

    private void HandleJump()
    {
        if (isJumping && !playerHealth.isDead && !gameState.isPaused)
        {
            isWalking = false;
            myRigidBody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isJumping = false;
            animator.SetBool(IS_JUMPING, isJumping);
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
            playerSFX.PlayOneShot(coinCollectedAudio);
            Destroy(other.gameObject);
            GameManager.Instance.AddCoin();
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

        myRigidBody.AddForce(delayedForceToApply, ForceMode.VelocityChange);
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
