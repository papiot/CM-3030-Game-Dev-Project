using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask; // Renamed for clarity

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

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        gameScore = 0;
    }

    void Update()
    {
        horInput = Input.GetAxis("Horizontal");
        verInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            isJumping = true;
        }

        RotatePlayerToMouseCursor();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        HandleJump();
    }

    private void MovePlayer()
    {
        Vector3 moveDir = new Vector3(horInput * moveForce, myRigidBody.velocity.y, verInput * moveForce);
        myRigidBody.velocity = moveDir;
        isWalking = horInput != 0 || verInput != 0;

        animator.SetBool(IS_WALKING, isWalking);
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
        return Physics.CheckSphere(groundCheck.position, 0.1f, groundMask);
    }


    // collecting coins.. can be updated for ammo or health points
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
}


// OLD MOVEMENT LOGIC, USES ONLY THE KEYBOARD FOR CONTROLLING PLAYER MOVEMENT & DIRECTION

//public class PlayerScript : MonoBehaviour
//{
//    [SerializeField] private Transform groundCheck;
//    [SerializeField] private LayerMask groundMask; // Renamed for clarity

//    private float jumpForce = 7f;
//    private bool isJumping = false;

//    private float horInput;
//    private float verInput;
//    [SerializeField] float moveForce = 3f;

//    private Rigidbody myRigidBody;
//    private int gameScore;

//    private bool isWalking = false;
//    private const string IS_WALKING = "IsWalking";
//    [SerializeField] Animator animator = null;

//    void Start()
//    {
//        myRigidBody = GetComponent<Rigidbody>();
//        gameScore = 0;
//    }

//    // this is where we place all key detection code for play behavior.
//    // Update is called once per frame, varies per machine
//    void Update()
//    {
//        horInput = Input.GetAxis("Horizontal");
//        verInput = Input.GetAxis("Vertical");

//        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
//        {
//            isJumping = true;
//        }
//    }

//    // fixed update is where we place all the physics movements of our game
//    // this is called 100 times per second
//    private void FixedUpdate()
//    {
//        MovePlayer();
//        HandleJump();
//    }


//    private void MovePlayer()
//    {
//        Vector3 moveDir = new Vector3(horInput * moveForce, myRigidBody.velocity.y, verInput * moveForce);
//        myRigidBody.velocity = moveDir;
//        isWalking = moveDir != Vector3.zero;

//        if (horInput != 0 || verInput != 0)
//        {
//            Vector3 horizontalMoveDir = new Vector3(horInput, 0, verInput);
//            transform.forward = horizontalMoveDir; // Rotate to face movement direction
//        }

//        animator.SetBool(IS_WALKING, isWalking);
//    }


//    private void HandleJump()
//    {
//        if (isJumping)
//        {
//            isWalking = false;
//            myRigidBody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
//            isJumping = false;
//        }
//    }


//    private bool IsGrounded()
//    {
//        return Physics.CheckSphere(groundCheck.position, 0.1f, groundMask);
//    }



//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.layer == 6)
//        {
//            gameScore += 1;
//            GetComponent<AudioSource>().Play();
//            Destroy(other.gameObject);
//            Debug.Log("Game Score = " + gameScore);
//        }
//    }
//}