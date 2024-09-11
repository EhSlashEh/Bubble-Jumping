using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promtMessage;

    // Function will be called from player
    public void BaseInteract()
    {
        Interact();
    }

    // To overwrite
    protected virtual void Interact()
    {
    }
}
