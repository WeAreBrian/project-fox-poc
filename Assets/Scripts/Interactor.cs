using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public IEnumerable<InteractableObject> Interactables => m_Interactables;

    private List<InteractableObject> m_Interactables = new List<InteractableObject>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var interactable = collision.GetComponent<IInteractable>();

		if (interactable != null)
		{
			var interactableObject = new InteractableObject()
			{
				Interactable = interactable,
				GameObject = collision.gameObject
			};

			m_Interactables.Add(interactableObject);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		var interactable = collision.GetComponent<IInteractable>();

		if (interactable != null)
		{
			m_Interactables.RemoveAll(x => x.Interactable == interactable);
		}
	}

	public InteractableObject GetInteractable()
	{
		return m_Interactables
			.Where(x => x.GameObject != null && x.Interactable.IsActive)
			.OrderByDescending(x => x.Interactable.Priority)
			.ThenBy(x => (x.GameObject.transform.position - transform.position).sqrMagnitude)
			.FirstOrDefault();
	}
	
	public bool Interact()
	{
		var interactable = GetInteractable();

		if (interactable == null)
		{
			return false;
		}

		interactable.Interact();

		return true;
	}
}
