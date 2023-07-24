using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Recall : MonoBehaviour
{
	private Anchor m_Anchor;
	private AnchorHolder m_Holder;
	private InputAction m_RecallAction;

	[SerializeField]
	private AudioClip m_RecallSound;

	private void Awake()
	{
		m_Anchor = FindObjectOfType<Anchor>();
		m_Holder = GetComponent<AnchorHolder>();
		m_RecallAction = GetComponent<PlayerInput>().actions["Recall"];
		m_RecallAction.performed += ctx => Activate();
	}

	private void Activate()
	{
		AudioController.PlaySound(m_RecallSound, 1, 1, MixerGroup.SFX);
		m_Anchor.transform.position = transform.position;
		m_Holder.ForcePickup();
	}
}
