using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    private float currentSpeed;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    private bool isGrounded;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.2f;

    private Vector3 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = walkSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move the player based on input
        MovePlayer();
    }

    // Called from InputManager to process movement
    public void ProcessMove(Vector2 input)
    {
        // Convert input from 2D to 3D movement
        movementInput = new Vector3(input.x, 0, input.y);
    }

    // Function to handle player movement
    private void MovePlayer()
    {
        // Apply movement based on input and speed
        Vector3 move = transform.TransformDirection(movementInput) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    // Called from InputManager when the player jumps
    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Called from InputManager to toggle sprinting
    public void SetSprinting(bool isSprinting)
    {
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }
}
