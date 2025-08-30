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
    bool isRunning;
    bool isExhausted;
    float stamina;
    Rigidbody rb;

    [HideInInspector]
    public PlayerProfile playerProfile;
    [HideInInspector]
    public PlayerBob playerBob;
    [HideInInspector]
    public PlayerManager playerManager;

    [Header("Physics References")]

    // Ok not needed now but could be useful depending on a new model
    public Transform orientation;
    public Transform groundCheckOrigin;
    public LayerMask groundLayer;

    // Make sure it's >=1 since Raycast point is 0,1,0
    public float groundCheckDistance = 1.1f;
    public float airMultiplier = 0.5f;
    public float counterStrafeMultiplier = 2f;
    public float fallDamageMultiplier = 2.3f;
    public bool canRun;

    [Header("Audio")]
    public List<AudioClip> runningSounds;
    public AudioClip jumpSound; 
    public float stepRate = 0.5f;

    private AudioSource audioSource;
    private float nextStepTime = 0f;


    private double lastYVelocity;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the object.");
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Audio Source not found.");
        }
        stamina = playerProfile.maxStamina;
    }

    void Update()
    {
        if (Time.timeScale != 0)
            GetInput();


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true && Time.timeScale != 0)
        {
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }

            Jump();
        }
        isRunning = Input.GetKey(KeyCode.LeftShift) && !isExhausted && canRun;
        HandleStamina();
    }

    Boolean wasGroundedLastFrame;
    float fallStartHeight;
    private void FixedUpdate()
    {
        // isGrounded = Physics.Raycast(groundCheckOrigin.position, Vector3.down, groundCheckDistance, groundLayer);
        // MovePlayer();
        // if (isGrounded && lastYVelocity < -10)
        // {
        //     playerManager.TakeDamage((int)(Mathf.Abs((float)lastYVelocity) * fallDamageMultiplier));
        // }
        // lastYVelocity = rb.linearVelocity.y;

        bool groundedNow = Physics.Raycast(groundCheckOrigin.position, Vector3.down, groundCheckDistance, groundLayer);

        if (!wasGroundedLastFrame && groundedNow)
        {
            // Just landed
            float fallDistance = fallStartHeight - transform.position.y;

            if (fallDistance > 3f) // Adjust this threshold as needed
            {
                float damage = (fallDistance - 3f) * fallDamageMultiplier;
                playerManager.TakeDamage((int)damage, false);
            }
        }

        if (wasGroundedLastFrame && !groundedNow)
        {
            // Just left the ground, start tracking fall
            fallStartHeight = transform.position.y;
        }

        wasGroundedLastFrame = groundedNow;
        isGrounded = groundedNow;

        MovePlayer();
        lastYVelocity = rb.linearVelocity.y;
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }
    public void ResetMovement()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        horizontalInput = 0f;
        verticalInput = 0f;

        isRunning = false;
        isExhausted = false;
    }
    private void MovePlayer()
    {

        Vector3 intendedMoveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float runningMultiplier = isRunning ? 15f : 1f;
        float sidewaysMultiplier = Mathf.Abs(horizontalInput) > 0.01f ? 0.6f : 1f;

        // Ground movement
        if (isGrounded)
        {
            float directionDot = Vector3.Dot(intendedMoveDirection.normalized, horizontalVelocity.normalized);
            HandleBob();
            if (isRunning)
            {
                stamina -= Time.deltaTime;
            }
            // If we are trying to move in the opposite direction, apply the multiplier
            // Gives a nice counter-strafe effect
            if (directionDot < -0.1f) // A sharp turn
            {
                rb.AddForce(intendedMoveDirection.normalized * playerProfile.movementForce * runningMultiplier * sidewaysMultiplier * counterStrafeMultiplier, ForceMode.Acceleration);
            }
            else // Otherwise, use normal force
            {
                rb.AddForce(intendedMoveDirection.normalized * playerProfile.movementForce * runningMultiplier * sidewaysMultiplier, ForceMode.Acceleration);
            }
        }
        // Air movement
        else if (!isGrounded)
        {
            rb.AddForce(intendedMoveDirection.normalized * playerProfile.movementForce * airMultiplier, ForceMode.Acceleration);
        }

        // Braking force, to prevent sliding/drifting
        if (isGrounded && intendedMoveDirection == Vector3.zero)
        {
            // Calculate a force opposite to our current velocity
            Vector3 counterForce = new Vector3(-rb.linearVelocity.x, 0, -rb.linearVelocity.z);
            rb.AddForce(counterForce * playerProfile.brakingForce * Time.fixedDeltaTime);
        }

        // Speed limiter since we're using addForce
        horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
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
                    audioSource.PlayOneShot(randomClip, 0.5f);
                }
            }
        }
    }

    private void HandleStamina()
    {
        if (stamina < 0)
        {
            isExhausted = true;
        }
        if (isExhausted)
        {
            stamina += 3 * Time.deltaTime;
            if (stamina >= playerProfile.maxStamina)
            {
                isExhausted = false;
            }
        }
    }

    private void HandleBob()
    {
        if (Mathf.Abs(horizontalInput) > 0.01f || Mathf.Abs(verticalInput) > 0.01f)
        {
            playerBob.DoWalkingHeadBob(isRunning ? 1.2f : 0.6f);
            if (horizontalInput != 0)
            {
                playerBob.DoSidewaysTilt(horizontalInput < -0.01f ? 1 : -1);
            }
            else if (verticalInput != 0)
            {
                playerBob.DoStraightTilt(verticalInput < -0.01f ? 1 : -1);
            }
        }
        else
        {
            playerBob.Neutral();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * playerProfile.jumpForce, ForceMode.Impulse);
    }
}
