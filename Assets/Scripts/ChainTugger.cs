using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainTugger : MonoBehaviour
{
	public float Cooldown = 1.5f;
	public float TugForce = 600;
	public float AnchorDislodgeTravelTime = 0.85f;

	private IdealChain m_Chain;
	private Timer m_Cooldown;
	private Anchor m_Anchor;
	private Grounded m_Grounded;
	private AnchorHolder m_Holder;

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
				m_Chain.Player.AddForce(m_Chain.PlayerToPendulum * TugForce);
				break;
			case AnchorState.Lodged:
				var dislodgeVelocity = CalculateInitialVelocity(m_Chain.Anchor.position, m_Chain.AnchorPendulumPoint, AnchorDislodgeTravelTime);
				m_Anchor.Dislodge(dislodgeVelocity);
				LeanTween.delayedCall(AnchorDislodgeTravelTime, () => m_Holder.GrabAnchor());
				break;
			case AnchorState.Free:
				if (m_Grounded.OnGround)
				{
					m_Chain.Anchor.AddForce(m_Chain.AnchorToPendulum * TugForce, ForceMode2D.Impulse);
				}
				else
				{
					m_Chain.Player.AddForce(m_Chain.PlayerToPendulum * TugForce, ForceMode2D.Impulse);
					m_Chain.Anchor.AddForce(m_Chain.AnchorToPendulum * TugForce, ForceMode2D.Impulse);
				}
				break;
		}

		m_Cooldown.Start();
	}

	private void Update()
	{
		m_Cooldown.Tick();
	}

	private static Vector2 CalculateInitialVelocity(Vector2 currentPosition, Vector2 targetPosition, float time)
	{
		var displacement = targetPosition - currentPosition;
		var initialVelocity = displacement / time - 0.5f * Physics2D.gravity.y * time * Vector2.up;

		return initialVelocity;
	}
}
