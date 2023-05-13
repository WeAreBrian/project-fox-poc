using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public IEnumerable<InteractableObject> Interactables => m_Interactables;

    private List<InteractableObject> m_Interactables = new List<InteractableObject>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var interactable = collision.GetComponent<IInteractable>();

		if (interactable == null)
		{
			return;
		}

		var interactableObject = new InteractableObject()
		{
			Interactable = interactable,
			GameObject = collision.gameObject
		};

		m_Interactables.Add(interactableObject);

		// To ensure that only one listener will be active per input action
		interactable.Input.action.performed -= OnInputActionPerformed;
		interactable.Input.action.performed += OnInputActionPerformed;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		var interactable = collision.GetComponent<IInteractable>();

		if (interactable != null)
		{
			m_Interactables.RemoveAll(x => x.Interactable == interactable);
		}
	}

	private void OnInputActionPerformed(InputAction.CallbackContext context)
	{
		Interact(context.action);
	}
	
	public IEnumerable<InteractableObject> GetInteractablesByInput()
	{
		return m_Interactables
			.Where(x => x.GameObject != null && x.Interactable.IsActive)
			.GroupBy(x => x.Interactable.Input)
			.Select(x => x.OrderByDescending(x => x.Interactable.Priority).ThenBy(x => (x.GameObject.transform.position - transform.position).sqrMagnitude).First());
	}
	
	public bool Interact(InputAction action)
	{
		var interactable = GetInteractablesByInput()
			.FirstOrDefault(x => x.Interactable.Input.action == action);

		if (interactable == null)
		{
			return false;
		}

		interactable.Interact();

		return true;
	}
}
