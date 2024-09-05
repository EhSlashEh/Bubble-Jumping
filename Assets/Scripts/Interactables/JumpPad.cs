using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : Interactable
{

    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        if (player != null)
        {
            // Get the PlayerMovement component from the player
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

            // Check if the player is in range (optional range check can be added)
            if (playerMovement != null)
            {
                // Call the Explode method and pass the JumpPad's position
                playerMovement.Explode(transform.position);
            }
        }
    }
}
