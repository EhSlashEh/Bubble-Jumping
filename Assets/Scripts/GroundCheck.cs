using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public PlayerController controller;
    public GameObject bullet;

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject == controller.gameObject)
        {
            return;
        }
        if (col.gameObject == bullet)
        {
            return;
        }

        controller.SetGrounded(true);
    }
    
}
