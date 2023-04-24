using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ChainClimber : MonoBehaviour
{
	public float ClimbSpeed = 4.5f;
	public float MountMaxAngle = 15;

	public Vector2 LinkAnchor => new Vector2(0, Mathf.Repeat(m_Chain.Length, m_PhysicsChain.LinkAnchorDistance) - m_PhysicsChain.LinkAnchorOffset);
	public int LinkIndex => Mathf.FloorToInt(m_Chain.Length / m_PhysicsChain.LinkAnchorDistance);
	public Rigidbody2D Link => m_PhysicsChain.GetLink(LinkIndex);
	public bool Mounted => m_DistanceJoint != null;
	public bool CanMount => m_Chain.PlayerTension > 0 ||
		Vector2.Angle(Vector2.down, (m_Chain.Player.position - m_Chain.PlayerPendulum.position).normalized) < MountMaxAngle;

	private Grounded m_Grounded;
	private IdealChain m_Chain;
	private PhysicsChain m_PhysicsChain;
	private float m_ClimbDirection;
	private float m_MountDistance;
	private DistanceJoint2D m_DistanceJoint;

	public void Mount()
	{
		if (Mounted)
		{
			return;
		}

		if (!CanMount)
		{
			return;
		}

		m_DistanceJoint = gameObject.AddComponent<DistanceJoint2D>();
		m_DistanceJoint.autoConfigureConnectedAnchor = false;
		m_DistanceJoint.autoConfigureDistance = false;
		m_DistanceJoint.anchor = Vector2.zero;
		m_DistanceJoint.connectedAnchor = Vector2.zero;
		m_DistanceJoint.connectedBody = m_Chain.PlayerPendulum;
		m_DistanceJoint.distance = Vector2.Distance(transform.position, m_Chain.PlayerPendulum.position);

		m_MountDistance = m_Chain.Length;
	}

	public void Dismount()
	{
		if (!Mounted)
		{
			return;
		}

		Destroy(m_DistanceJoint);
		m_DistanceJoint = null;
	}

	public void Climb(float direction)
	{
		m_ClimbDirection = direction;
	}

	private void OnJump()
	{
		Dismount();
	}

	private void OnMount()
	{
		Mount();
	}

	private void OnClimb(InputValue value)
	{
		var direction = value.Get<float>();
		Climb(direction);
	}

	private void Awake()
	{
		m_Grounded = GetComponent<Grounded>();

		m_Chain = FindObjectOfType<IdealChain>();
		m_PhysicsChain = FindObjectOfType<PhysicsChain>();
	}

	private void UpdateDistanceJoint()
	{
		if (!Mounted)
		{
			return;
		}

		m_DistanceJoint.distance = m_MountDistance - (m_Chain.Length - Vector2.Distance(transform.position, m_Chain.PlayerPendulumPoint));
		m_DistanceJoint.connectedBody = m_Chain.PlayerPendulum;
	}

	private void FixedUpdate()
	{
		if (!Mounted)
		{
			return;
		}

		if (m_Grounded.OnGround || m_Chain.Anchor.bodyType == RigidbodyType2D.Dynamic)
		{
			Dismount();
			return;
		}

		m_MountDistance -= m_ClimbDirection * ClimbSpeed * Time.fixedDeltaTime;
		m_MountDistance = Mathf.Clamp(m_MountDistance, 0, m_Chain.MaxLength);

		UpdateDistanceJoint();

		Link.MovePosition(transform.position);
	}

	private void OnDrawGizmosSelected()
	{
		if (!Mounted)
		{
			return;
		}

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(Link.transform.TransformPoint(LinkAnchor), 0.2f);
	}
}
