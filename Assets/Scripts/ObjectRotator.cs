using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float rotationSpeed = 100f;  // Speed of rotation
    public float bounceHeight = 0.5f;   // Height of the bounce
    public float bounceSpeed = 2f;      // Speed of the bounce

    private Vector3 startPosition;      // Starting position of the object

    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotate the object around its Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Calculate the new Y position using a sine wave for smooth bouncing
        float newY = startPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

        // Apply the new Y position to the object
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
