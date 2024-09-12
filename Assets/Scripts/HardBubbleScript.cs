using System.Collections;
using UnityEngine;

public class HardBubbleScript : MonoBehaviour
{
    public float stayDuration = 5.0f; // Duration of the stick
    public float fadeDuration = 5.0f; // Duration of the fade out
    public float initialSpeed = 0.2f; // Initial speed of the object
    public float acceleration = 0.5f; // Rate at which the object speeds up

    private Material objectMaterial;
    private Color initialColor;

    // Start is called before the first frame update
    void Start()
    {
        // Get the material of the object (assumed to have a Renderer component)
        objectMaterial = GetComponent<Renderer>().material;
        initialColor = objectMaterial.color;

        // Start the coroutine to handle the sequence
        StartCoroutine(BubbleLifecycle());
    }

    IEnumerator BubbleLifecycle()
    {
        // Step 1: Do nothing for stayDuration
        yield return new WaitForSeconds(stayDuration);

        // Step 2: Move the object down and fade it out over the next few seconds
        float elapsedTime = 0;
        float currentSpeed = initialSpeed;

        while (elapsedTime < fadeDuration)
        {
            // Apply acceleration to speed up over time
            currentSpeed += acceleration * Time.deltaTime;

            // Move the object down each frame
            transform.position -= new Vector3(0, currentSpeed * Time.deltaTime, 0);

            // Fade out by decreasing the alpha of the object's material
            float alpha = Mathf.Lerp(0.5f, 0, elapsedTime / fadeDuration);
            objectMaterial.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Step 3: Destroy the object after fading
        Destroy(gameObject);
    }
}
