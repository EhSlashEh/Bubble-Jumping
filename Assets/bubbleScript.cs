using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubbleScript : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        playerController.BubbleJump(transform.position);

        Destroy(gameObject);
    }
}
