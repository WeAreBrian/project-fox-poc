using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour, IInteractable
{
	[SerializeField]
	private UnityEvent m_Interacted;
	[SerializeField]
	private int m_Priority;
	[SerializeField]
	private bool m_IsActive = true;
	[SerializeField]
	private InputActionReference m_Input;

	public InputActionReference Input => m_Input;
	public int Priority => m_Priority;
	public bool IsActive => m_IsActive;

	public void Interact()
	{
		m_Interacted?.Invoke();
	}
}
