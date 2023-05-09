using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPrompts : MonoBehaviour
{
	[Tooltip("The number of world units that the prompt will be offset from the interactable object.")]
	[SerializeField]
	private Vector2 m_PromptOffset = new Vector2(0, 2);
	[SerializeField]
	private GameObject m_PromptPrefab;
	private Interactor m_PlayerInteractor;
	private InputPrompt m_Prompt;

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

		var interactable = m_PlayerInteractor.GetInteractable();

		if (interactable == null)
		{
			if (m_Prompt != null)
			{
				m_Prompt.Dispose();
				m_Prompt = null;
			}
		}
		else
		{
			if (m_Prompt != null)
			{
				if (interactable == m_Prompt.Interactable)
				{
					return;
				}

				m_Prompt.Dispose();
				m_Prompt = null;
			}

			m_Prompt = Instantiate(m_PromptPrefab, transform).GetComponent<InputPrompt>();
			m_Prompt.Interactable = interactable;
			m_Prompt.Offset = m_PromptOffset;
		}
	}
}
