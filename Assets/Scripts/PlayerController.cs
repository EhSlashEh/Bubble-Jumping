using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Game Objects")]
    public Rigidbody rb;
    public GameObject camHolder;
    public TextMeshProUGUI promtText;
    public GameObject bubbleBullet;

    [Header("Player Movement Forces")]
    public float speed = 10f;
    public float sensitivity = 0.1f;
    public float jumpForce = 3f;
    public float airControl = 0.15f;

    [Header("Firing")]
    public float bubbleBulletSpeed = 30f;
    public float fireCooldown = 1f;
    private float lastFireTime = 0f;

    [Header("Bubble Power")]
    public float bubbleForce = 100f;
    public float minForce = 0.2f;
    public float maxForce = 0.8f;

    [Header("Interactions")]
    public float raycastDistance = 10f;
    public LayerMask interactableLayer;

    // Other vars
    private Vector2 move, look;
    private float lookRotation;
    private bool isGrounded;
    private bool isBubbleJumping;

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
        if (context.performed)
        {
            Jump();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Fire();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Interact();
        }
    }

    // Timings
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckForInteraction();
        CheckGrounded();
    }

    void FixedUpdate()
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
        // Find target velocity based on player input
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity = transform.TransformDirection(targetVelocity) * speed;

        // Calculate the current velocity without vertical component
        Vector3 currentHorizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // When airborne, blend the target velocity with the current velocity to retain momentum        
        if (!isGrounded)
        {
            // Blend the current velocity with the new target velocity to maintain momentum
            targetVelocity = Vector3.Lerp(currentHorizontalVelocity, targetVelocity, airControl);
        }
        

        // Calculate velocity change
        Vector3 velocityChange = (targetVelocity - currentHorizontalVelocity);

        // Apply the velocity change
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
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
        camHolder.transform.localRotation = Quaternion.Euler(lookRotation, 0f, 0f);
    }

    private void Jump()
    {
        if (!isGrounded)
        {
            return;
        }

        Vector3 jumpForces = Vector3.up * jumpForce;

        rb.AddForce(jumpForces, ForceMode.VelocityChange);
    }

    private void Fire()
    {
        if (Time.time - lastFireTime < fireCooldown)
        {
            Debug.Log("Firing is on cooldown.");
            return; // If not enough time has passed, don't fire
        }

        lastFireTime = Time.time; // Update the last fire time

        // Instantiate a new bubble bullet at the camera's position and orientation
        GameObject newBubble = Instantiate(bubbleBullet, camHolder.transform.position, camHolder.transform.rotation);

        // Get the Rigidbody component of the bubble bullet to apply force to it
        Rigidbody bubbleRb = newBubble.GetComponent<Rigidbody>();

        // Set the velocity of the bubble bullet to move forward relative to the camera's direction
        bubbleRb.velocity = camHolder.transform.forward * bubbleBulletSpeed;
    }

    private void Interact()
    {
        RaycastHit hit = CheckForInteraction();

        if (hit.collider != null)
        {
            // Get the Interactable component from the hit collider
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                // Log the interaction
                Debug.Log("Interacting with: " + hit.collider.name);

                // Perform interaction logic
                interactable.BaseInteract();
            }
            else
            {
                Debug.LogWarning("No Interactable component found on: " + hit.collider.name);
            }
        }
    }

    public void BubbleJump(Vector3 bubbleOrigin)
    {
        // Calculate the direction and distance from the gameObject to the bubbleOrigin
        Vector3 direction = (transform.position - bubbleOrigin).normalized;
        float distance = Vector3.Distance(transform.position, bubbleOrigin);

        if (distance > 8)
        {
            return;
        }

        // Calculate the strength of the force based on the distance
        float distancePower = Mathf.Clamp(1 / distance, minForce, maxForce);

        // Apply force based on direction and distance
        Vector3 bubbleJumpForces = direction * distancePower * bubbleForce;

        // Log the values
        Debug.Log("Direction: " + direction + " | Distance: " + distance + " | Distance Power: " + distancePower + " | Bubble Jump Forces: " + bubbleJumpForces);

        // Set isGrounded to false for 0.3 seconds
        if (!isBubbleJumping)
        {
            StartCoroutine(HandleBubbleJump());
        }

        // Apply force to player
        rb.AddForce(bubbleJumpForces, ForceMode.VelocityChange);
    }

    private IEnumerator HandleBubbleJump()
    {
        isBubbleJumping = true;
        isGrounded = false;

        yield return new WaitForSeconds(0.3f);

        isBubbleJumping = false;
    }

    // Ground
    private void CheckGrounded()
    {
        float groundCheckRadius = 0.3f;
        Vector3 origin = transform.position + Vector3.down * (groundCheckRadius + 0.5f);

        // Get all colliders within the sphere
        Collider[] colliders = Physics.OverlapSphere(origin, groundCheckRadius);

        // Check if any of the colliders are not tagged as "Player"
        if (!isBubbleJumping)
        {
            isGrounded = false;
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    continue;
                }

                // If we find a collider that is not tagged as "Player", the player is grounded
                isGrounded = true;
                break;
            }
        }

        // Draw a debug sphere to visualize the ground check
        Debug.DrawRay(origin, Vector3.down * groundCheckRadius, Color.red);
    }

    // Look Interactions
    private RaycastHit CheckForInteraction()
    {
        // Perform the raycast from the center of the camera
        Ray ray = new Ray(camHolder.transform.position, camHolder.transform.forward);
        RaycastHit hit;

        // Check if the raycast hits something
        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            // Try to get the Interactable component from the hit object
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                promtText.text = interactable.promtMessage; // Use the promptMessage from the Interactable component
            }
            else
            {
                promtText.text = "Looking at: " + hit.collider.name; // Fallback if no Interactable component is found
            }
        }
        else
        {
            promtText.text = "";
            hit = default; // Reset hit if nothing was found
        }

        return hit;
    }

}
