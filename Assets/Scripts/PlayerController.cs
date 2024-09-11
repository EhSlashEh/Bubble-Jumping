using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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

    [Header("Bubble Power")]
    public float bubbleForce = 100f;
    public float maxForce = 20f;

    [Header("Interactions")]
    public float raycastDistance = 10f;
    public LayerMask interactableLayer;

    private Vector2 move, look;
    private float lookRotation;
    private bool isGrounded;

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
        Vector3 jumpForces = Vector3.zero;

        if (isGrounded)
        {
            jumpForces = Vector3.up * jumpForce;
        }

        rb.AddForce(jumpForces, ForceMode.VelocityChange);

        isGrounded = false;
    }

    private void Fire()
    {
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

        // Calculate the strength of the force based on the distance
        float distancePower = Mathf.Clamp(1 / distance, 0, maxForce);

        // Apply force based on direction and distance
        Vector3 bubbleJumpForces = direction * distancePower * bubbleForce;

        // Log the values
        Debug.Log("Distance: " + distance);
        Debug.Log("Distance Power: " + distancePower);
        Debug.Log("Bubble Jump Forces: " + bubbleJumpForces);

        // Apply force to player
        rb.AddForce(bubbleJumpForces, ForceMode.VelocityChange);
    }

    // States
    public void SetGrounded(bool state)
    {
        isGrounded = state;
    }

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
