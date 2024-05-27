using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class PlayerScript : MonoBehaviour
//{
//    [SerializeField] private Transform groundCheck;
//    [SerializeField] private LayerMask playerMask;

//    private float jumpH;
//    private bool isJumping = false;


//    private float horInput;
//    private float verInput;
//    private float moveForce;

//    private Rigidbody myRigidBody;

//    private int gameScore;

//    // Start is called before the first frame update
//    void Start()
//    {
//        myRigidBody = GetComponent<Rigidbody>();
//        moveForce = 3;

//        gameScore = 0;
//    }

//    // this is where we place all key detection code for play behavior.
//    // Update is called once per frame, varies per machine
//    void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.Space))
//        {
//            isJumping = true;
//        }

//        horInput = Input.GetAxis("Horizontal");
//        verInput = Input.GetAxis("Vertical");
//    }


//    // fixed update is where we place all the physics movements of our game
//    // this is called 100 times per second
//    private void FixedUpdate()
//    {
//        // check if jumping is true & that player is grounded by detecting num of collision points
//        if (isJumping && Physics.OverlapSphere(groundCheck.position, 0.1f, playerMask).Length > 0)
//        {
//            isJumping = false;
//            jumpH = 7;

//            myRigidBody.AddForce((Vector3.up * jumpH),ForceMode.VelocityChange);
//        }

//        // Move the player based on the direction the player is facing and the mouse position

//        Vector3 moveDir = new Vector3(horInput * moveForce, myRigidBody.velocity.y, verInput * moveForce);
//        Vector3 horizontalMoveDir = new Vector3(moveDir.x, 0, moveDir.z); // Ignore vertical component for rotation

//        bool isWalking = horizontalMoveDir != Vector3.zero;

//        myRigidBody.velocity = moveDir;

//        // using horizontal input to create horizontal motion
//        if (isWalking)
//        {
//            transform.forward = horizontalMoveDir * Time.deltaTime;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if(other.gameObject.layer == 6)
//        {
//            gameScore += 1;
//            GetComponent<AudioSource>().Play();

//            Destroy(other.gameObject);

//            Debug.Log("Game Score =  " + gameScore.ToString());
//            //Debug.Log("Super Jumps = " + superJumps.ToString());


//        }
//    }

//}

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask; // Renamed for clarity

    private float jumpForce = 7f;
    private bool isJumping = false;

    private float horInput;
    private float verInput;
    [SerializeField] float moveForce = 3f;

    private Rigidbody myRigidBody;
    private int gameScore;

    private bool isWalking = false;
    private const string IS_WALKING = "IsWalking";
    [SerializeField] Animator animator = null;

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
        isWalking = moveDir != Vector3.zero;

        if (horInput != 0 || verInput != 0)
        {
            Vector3 horizontalMoveDir = new Vector3(horInput, 0, verInput);
            transform.forward = horizontalMoveDir; // Rotate to face movement direction
        }

        animator.SetBool(IS_WALKING, isWalking);
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