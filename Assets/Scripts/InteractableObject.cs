using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject
{
    public IInteractable Interactable;
    public GameObject GameObject;
	
    public void Deconstruct(out IInteractable interactable, out GameObject gameObject)
    {
        interactable = Interactable;
        gameObject = GameObject;
    }

    public void Interact()
    {
        Interactable.Interact();
    }
}
