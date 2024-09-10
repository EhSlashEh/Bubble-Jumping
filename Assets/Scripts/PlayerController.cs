using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Game Objects")]
    public Rigidbody rb;
    public GameObject camHolder;

    [Header("Floats")]
    public float speed;
    public float sensitivity;
    public float maxForce;
    public float jumpForce;

    private Vector2 move, look;
    private float lookRotation;
    private bool grounded;

    // Input calls
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    // Timings
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }
    
    void LateUpdate()
    {
        Look();
    }

    // Actions
    private void Move()
    {
        // Find target velocity
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity *= speed;

        // Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        // Calculate forces
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);


        // Limit force
        Vector3.ClampMagnitude(velocityChange, speed);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Look()
    {
        // Turn (horizontal)
        transform.Rotate(Vector3.up * look.x * sensitivity);

        // Look (Vertical)
        lookRotation += (-look.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);

        // Apply rotation
        // OLD: camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
        camHolder.transform.localRotation = Quaternion.Euler(lookRotation, 0f, 0f);
    }

    private void Jump()
    {
        Vector3 jumpForces = Vector3.zero;

        if (grounded)
        {
            jumpForces = Vector3.up * jumpForce;
        }

        rb.AddForce(jumpForces, ForceMode.VelocityChange);
    }

    public void BubbleJump(Vector3 bubbleOrigin)
    {
        // Calculate the direction and distance from the gameObject to the bubbleOrigin
        Vector3 direction = (transform.position - bubbleOrigin).normalized;
        float distance = Vector3.Distance(transform.position, bubbleOrigin);

        // Calculate the strength of the force based on the distance
        float forceMultiplier = Mathf.Clamp(1 / distance, 0, 50);

        // Apply force based on direction and distance
        Vector3 bubbleJumpForces = direction * forceMultiplier * 10;

        // Additional upwards force
        bubbleJumpForces += Vector3.up * 3; 

        // Apply force to player
        rb.AddForce(bubbleJumpForces, ForceMode.VelocityChange);
    }

    // States
    public void SetGrounded(bool state)
    {
        grounded = state;
    }
}
