using UnityEngine;

public class ResetPlayer : MonoBehaviour
{
    // Reference to the GameObject that defines the reset position
    public Transform resetTransform;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that triggered the collision has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Reset the player's position to the position of the resetTransform
            other.transform.position = resetTransform.position;

            // Optionally, reset the player's rotation to match the resetTransform's rotation
            other.transform.rotation = resetTransform.rotation;

            // Optionally, reset velocity if using Rigidbody
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
