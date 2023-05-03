using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ChainTugger : MonoBehaviour
{
	public float Cooldown = 1.5f;
	public float PlayerJumpOffset = 0.25f;
	public float PlayerTugSpeed = 10;
	public float AnchorDislodgeTravelTime = 0.85f;
	public float AnchorFreeTravelTime = 0.85f;

	private IdealChain m_Chain;
	private Timer m_Cooldown;
	private Anchor m_Anchor;
	private Grounded m_Grounded;
	private AnchorHolder m_Holder;

	[SerializeField]
	private AudioClip m_PlayerTugSound;
	[SerializeField]
	private AudioClip m_FullTugSound;

	private void Awake()
	{
		m_Chain = FindObjectOfType<IdealChain>();
		m_Anchor = FindObjectOfType<Anchor>();

		m_Cooldown = new Timer(Cooldown);

		m_Grounded = GetComponent<Grounded>();
		m_Holder = GetComponent<AnchorHolder>();
	}

	private void OnTug()
	{
		if (!m_Cooldown.Paused)
		{
			return;
		}

		switch (m_Anchor.State)
		{
			case AnchorState.Held:
				return;
			case AnchorState.Grounded:
				transform.position += new Vector3(0, PlayerJumpOffset);
				m_Chain.Player.velocity = m_Chain.PlayerToPendulum * PlayerTugSpeed;
				AudioController.PlaySound(m_PlayerTugSound, 0.4f, 1, MixerGroup.SFX);
				break;
			case AnchorState.Lodged:
				DislodgeAnchor();
				AudioController.PlaySound(m_FullTugSound, 1, 1, MixerGroup.SFX);
				break;
			case AnchorState.Free:
				if (!m_Grounded.OnGround)
				{
					var midpoint = (m_Chain.Anchor.position + m_Chain.Player.position) / 2;
					var travelTime = AnchorFreeTravelTime / 2;

					m_Chain.Anchor.velocity = CalculateInitialVelocity(m_Chain.Anchor.position, midpoint, travelTime);
					m_Chain.Player.velocity = CalculateInitialVelocity(m_Chain.Player.position, midpoint, travelTime);

					LeanTween.delayedCall(travelTime, () => m_Holder.GrabAnchor());
				}
				else
				{
					m_Chain.Anchor.velocity = CalculateInitialVelocity(m_Chain.Anchor.position, m_Chain.Player.position, AnchorFreeTravelTime);
					LeanTween.delayedCall(AnchorFreeTravelTime, () => m_Holder.GrabAnchor());
				}

				AudioController.PlaySound(m_FullTugSound, 1, 1, MixerGroup.SFX);
				break;
		}

		m_Cooldown.Start();
	}

	private void Update()
	{
		m_Cooldown.Tick();
	}

	private void DislodgeAnchor()
	{
		var velocity = CalculateInitialVelocity(m_Chain.Anchor.position, m_Chain.AnchorPendulumPoint, AnchorDislodgeTravelTime);

		m_Anchor.Dislodge(velocity);

		LeanTween.delayedCall(AnchorDislodgeTravelTime, () => m_Holder.GrabAnchor());
	}

	private static Vector2 CalculateInitialVelocity(Vector2 currentPosition, Vector2 targetPosition, float time)
	{
		var displacement = targetPosition - currentPosition;
		var initialVelocity = displacement / time - 0.5f * Physics2D.gravity.y * time * Vector2.up;

		return initialVelocity;
	}
}
