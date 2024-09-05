using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // message displated to player when looking
    public string promtMessage;

    // Function will be called form player
    public void BaseInteract()
    {
        Interact();
    }
    protected virtual void Interact()
    {
        // To overwrite
    }
}
