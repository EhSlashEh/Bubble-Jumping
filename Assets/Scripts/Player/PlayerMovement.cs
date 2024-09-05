using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // private CharacterController controller;
    private Vector3 playerVelocity;
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 3f;
    private bool isSprinting = false;

    // Controller V2
    private Rigidbody rb;

    // For knock back
    public float explosionForce = 10f;  // How strong the explosion is
    public float explosionRadius = 5f;  // Radius of the explosion

    // Start is called before the first frame update
    void Start()
    {
        // controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Get input from the horizontal and vertical axes (typically the arrow keys or WASD keys)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create a vector for movement
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Apply force to the Rigidbody to move it
        rb.AddForce(movement * speed);
    }

    // Receive the inputs for our InputManager.cs and apply them to our character controller
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        Debug.Log(moveDirection);

        // Apply movement speed based on whether sprinting is active or not
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        // Use currentSpeed for movement
        // controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        // rb code
        rb.AddForce(moveDirection * currentSpeed * Time.deltaTime);

        /*
        // Check if the player is grounded
        if (controller.isGrounded)
        {
            // Log grounded state
            // Debug.Log("Player is grounded.");

            // Reset vertical velocity if the player is on the ground
            playerVelocity.y = -1f; // Slight downward force to ensure the player stays grounded
        }
        else
        {
            // Log grounded state
            // Debug.Log("Player is NOT grounded.");

            // Apply gravity
            playerVelocity.y += Physics.gravity.y * Time.deltaTime;
        }
        */



        // Apply movement based on player velocity
        // controller.Move(playerVelocity * Time.deltaTime);
    }

    // Trigger the jump action
    public void Jump()
    {
        /*
        if (controller.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
        }
        */
    }

    // Method to set sprinting state
    public void SetSprinting(bool isSprinting)
    {
        this.isSprinting = isSprinting;
    }

    public void Explode(Vector3 explosionPosition)
    {
        // Calculate direction from explosion to character
        Vector3 direction = (rb.transform.position - explosionPosition).normalized;

        // Calculate distance from explosion
        float distance = Vector3.Distance(explosionPosition, rb.transform.position);

        // Apply force inversely proportional to distance
        float force = Mathf.Clamp01(1 - distance / explosionRadius) * explosionForce;

        // Create movement vector based on the force
        Vector3 explosionMovement = direction * force;

        // Apply movement to the character
        rb.AddForce(explosionMovement * Time.deltaTime);
    }

}
