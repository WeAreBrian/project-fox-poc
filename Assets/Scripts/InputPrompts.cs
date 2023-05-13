using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputPrompts : MonoBehaviour
{
	[Tooltip("The number of world units that the prompt will be offset from the interactable object.")]
	[SerializeField]
	private Vector2 m_PromptOffset = new Vector2(0, 2);
	[SerializeField]
	private GameObject m_PromptPrefab;
	private Interactor m_PlayerInteractor;
	private List<InputPrompt> m_Prompts = new List<InputPrompt>();

	private void Start()
	{
		m_PlayerInteractor = GameObject.FindWithTag("Player").GetComponentInChildren<Interactor>();
	}

	private void Update()
	{
		//if (m_Prompt != null)
		//{
		//	m_Prompt.transform.position = Camera.main.WorldToScreenPoint((Vector2)m_Prompt.Interactable.GameObject.transform.position + m_PromptOffset);
		//}

		var interactables = m_PlayerInteractor.GetInteractablesByInput();

		// Remove prompts that no longer exist
		for (var i = m_Prompts.Count - 1; i >= 0; i--)
		{
			var prompt = m_Prompts[i];

			if (!interactables.Contains(prompt.InteractableObject))
			{
				RemoveInputPrompt(prompt);
			}
		}

		// Create prompts that don't exist yet
		foreach (var interactable in interactables)
		{
			if (m_Prompts.Any(x => x.InteractableObject == interactable))
			{
				continue;
			}

			var prompt = CreateInputPrompt(interactable);
			m_Prompts.Add(prompt);
		}
	}

	private void RemoveInputPrompt(InputPrompt prompt)
	{
		prompt.Dispose();
		m_Prompts.Remove(prompt);
	}

	private InputPrompt CreateInputPrompt(InteractableObject interactable)
	{
		var prompt = Instantiate(m_PromptPrefab, transform).GetComponent<InputPrompt>();
		prompt.InteractableObject = interactable;
		prompt.Offset = m_PromptOffset;

		return prompt;
	}
}
