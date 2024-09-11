using System.Collections;
using UnityEngine;

public class bubbleScript : MonoBehaviour
{
    private GameObject player;
    private Renderer bubbleRenderer;
    private Rigidbody rb;
    private bool popping = false;

    private void Start()
    {
        // Automatically find the Player game object by tag
        player = GameObject.FindWithTag("Player");

        // Get the Renderer component for transparency control
        bubbleRenderer = GetComponent<Renderer>();

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the bubble object.");
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == player || popping)
        {
            return;
        }

        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                // Do stuff
                popping = true;
                Debug.Log("Pop");

                // Set velocity to zero
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                playerController.BubbleJump(transform.position);

                // Start the destruction sequence with scaling and transparency animation
                StartCoroutine(GrowAndFadeOut());
            }
            else
            {
                Debug.LogError("PlayerController component not found on Player object.");
            }
        }
        else
        {
            Debug.LogError("Player object not found. Make sure the Player has the correct tag.");
        }
    }

    // Coroutine to handle growth and transparency fading
    private IEnumerator GrowAndFadeOut()
    {
        float duration = 0.2f; // Duration of the effect in seconds
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * 6f; // Scale by 5 times
        float time = 0;

        Color initialColor = bubbleRenderer.material.color;

        while (time < duration)
        {
            time += Time.deltaTime;

            // Scale the object over time
            transform.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);

            // Fade out by reducing alpha
            Color newColor = initialColor;
            newColor.a = Mathf.Lerp(1f, 0f, time / duration);
            bubbleRenderer.material.color = newColor;

            yield return null;
        }

        // Destroy the object after the effect
        Destroy(gameObject);
    }
}
