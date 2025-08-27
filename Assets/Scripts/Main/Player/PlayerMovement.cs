using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    bool isGrounded;
    Rigidbody rb;

    [HideInInspector]
    public PlayerProfile playerProfile;

    [Header("Physics References")]

    // Ok not needed now but could be useful depending on a new model
    public Transform orientation;
    public Transform groundCheckOrigin;
    public LayerMask groundLayer;

    // Make sure it's >=1 since Raycast point is 0,1,0
    public float groundCheckDistance = 1.1f;
    public float airMultiplier = 0.5f;

    [Header("Audio")]
    public List<AudioClip> runningSounds;
    public AudioClip jumpSound; 
    public float stepRate = 0.5f;


    private AudioSource audioSource;
    private float nextStepTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the object.");
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource==null)
        {
            Debug.LogError("Audio Source not found.");
        }
    }

    void Update()
    {
        if (Time.timeScale != 0)
            GetInput();


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true && Time.timeScale!=0)
        {
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }

            Jump();
        }


    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(groundCheckOrigin.position, Vector3.down, groundCheckDistance, groundLayer);
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

        // Apply forces depending if airborne or not
        if (isGrounded)
            rb.AddForce(intendedMoveDirection.normalized * playerProfile.movementForce, ForceMode.Acceleration);
        else
            rb.AddForce(intendedMoveDirection.normalized * playerProfile.movementForce * airMultiplier, ForceMode.Acceleration);

        if (isGrounded && intendedMoveDirection == Vector3.zero)
        {
            // Calculate a force opposite to our current velocity
            Vector3 counterForce = new Vector3(-rb.linearVelocity.x, 0, -rb.linearVelocity.z);
            rb.AddForce(counterForce * playerProfile.brakingForce * Time.fixedDeltaTime);
        }

        // Speed limiter since we're using addForce
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > playerProfile.maxSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * playerProfile.maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }

        // Footsteps sound logic
        if (isGrounded && intendedMoveDirection.sqrMagnitude > 0.1f && Time.time >= nextStepTime)
        {
            // Set the time for the next allowed step
            nextStepTime = Time.time + stepRate;

            // Pick and play a random running sound
            if (runningSounds != null && runningSounds.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, runningSounds.Count);
                AudioClip randomClip = runningSounds[randomIndex];
                if (randomClip != null)
                {
                    audioSource.PlayOneShot(randomClip,0.5f);
                }
            }
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * playerProfile.jumpForce, ForceMode.Impulse);
    }
}
