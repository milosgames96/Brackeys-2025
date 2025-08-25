using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    bool isGrounded;
    Rigidbody rb;

    [Header("DEV Tweaks")]

    public float moveSpeed;
    public float jumpForce;
    public float acceleration;

    [Header("Physics References")]

    // Ok not needed now but could be useful depending on a new model
    public Transform orientation;
    public Transform groundCheckOrigin;
    public LayerMask groundLayer;

    // Make sure it's >=1 since Raycast point is 0,1,0
    public float groundCheckDistance = 1.1f;
    public float gravityCorrection = 9.81f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the object.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale!=0)
          GetInput();


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true && Time.timeScale!=0)
        {
            //Debug.Log("Jumping now!");
            Jump();
        }


    }

    private void FixedUpdate()
    {
        // Perform the raycast
        isGrounded = Physics.Raycast(groundCheckOrigin.position, Vector3.down, groundCheckDistance, groundLayer);

        //Debug.Log(isGrounded ? "Grounded" : "In Air");

        // DEBUG: Drawing the ray to see what's going on
        //Color rayColor = isGrounded ? Color.red : Color.green;
        //Debug.DrawRay(groundCheckOrigin.position, Vector3.down * groundCheckDistance, rayColor);

        MovePlayer();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

    }
    private void MovePlayer()
    {
        Vector3 intendedMoveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Vector3 targetVelocity = intendedMoveDirection.normalized * moveSpeed;

        targetVelocity.y = rb.linearVelocity.y;

        // Acceleration simulation
        rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    private void Jump()
    {

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
