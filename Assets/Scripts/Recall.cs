using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Recall : MonoBehaviour
{
	private RecallSlingshot m_RecallSlingshot;

	public delegate void Trigger();
	public static event Trigger activate;

	private Anchor m_Anchor;
	private AnchorHolder m_Holder;
	private Grounded m_Grounded;
	private ChainClimber m_ChainClimber;
	private Rigidbody2D m_Rigidbody;
	private Chain m_Chain;


	[SerializeField]
	private Color m_CooldownColour;
	private Material m_AnchorMaterial;

	private bool OnCooldown => m_AnchorMaterial.color == m_CooldownColour;

	[SerializeField]
	private AudioClip m_RecallSound;

	private void Awake()
	{
		m_RecallSlingshot = GetComponent<RecallSlingshot>();

		m_Anchor = FindObjectOfType<Anchor>();
		m_Holder = GetComponent<AnchorHolder>();
		m_Grounded = GetComponent<Grounded>();
		m_ChainClimber = GetComponent<ChainClimber>();
		m_Rigidbody = GetComponent<Rigidbody2D>();
		m_Chain = FindObjectOfType<Chain>();

		m_Grounded.Landed.AddListener(ResetRecall);
		m_AnchorMaterial = m_Anchor.GetComponentInChildren<SpriteRenderer>().material;
		m_Anchor.StateChanged.AddListener(AnchorStateChange);
	}



	private void OnRecall()
	{
		if (!OnCooldown && !m_Holder.HoldingAnchor)
		{
			m_ChainClimber.Dismount();
			
			//Need to activate the slingshot BEFORE the anchor moves!
			m_RecallSlingshot?.TrySlingshot();
			AudioController.PlaySound(m_RecallSound, 1, 1, MixerGroup.SFX);
			m_Holder.ForcePickup();

			if (!m_Grounded.OnGround)
			{
				m_AnchorMaterial.color = m_CooldownColour;
			}
		}
	}

	private void AnchorStateChange(AnchorState state)
	{
		if (state == AnchorState.Lodged)
		{
			ResetRecall();
		}
	}

	private void ResetRecall()
	{
		m_AnchorMaterial.color = Color.white;
	}
}
