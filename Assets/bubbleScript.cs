using UnityEngine;

public class bubbleScript : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        // Automatically find the Player game object by tag
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == player)
        {
            return;
        }

        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.BubbleJump(transform.position);
                Destroy(gameObject);
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
}
