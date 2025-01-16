using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed;


    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private Respawn respawn;
    bool readyToJump = true;

    [SerializeField] private float playerHeigth;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundDrag;
    bool grounded;

    [SerializeField] private Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDircection;

    private Rigidbody rb;

    [Header("KeyBinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    public bool isAlive = true;
    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        respawn = GameObject.FindGameObjectWithTag("Logic").GetComponent<Respawn>();
    }

    private void FixedUpdate()
    {
        if (isAlive && !isPaused)
        {
            MovePlayer();
        }

        if (transform.position.y < -50) { 
            isAlive = false;
            respawn.Died();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeigth * 0.5f + 0.2f, whatIsGround);
        
        MyInput();
        SpeedControll();

        //hande drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        //when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded && isAlive && !isPaused)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDircection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDircection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded)
        {
            rb.AddForce(moveDircection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

    }

    private void SpeedControll()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //controll max velocity
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
