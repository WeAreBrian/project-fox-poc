using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChainMovement : MonoBehaviour
{
	public float ClimbSpeed = 4.5f;
	public float MountRadius = 1;
	public float DismountVelocity = 5;
	public float DismountMaxAngle = 60;
	public bool MountFurthestLink = true;

	public bool Mounted => m_Attachment != null;

	private Chain m_Chain;
	private int m_CurrentLinkIndex;
	private DistanceJoint2D m_Attachment;
	private Rigidbody2D m_RigidBody;
	private float m_DismountDirection;
	private List<Collider2D> m_MountableLinks = new List<Collider2D>();
	private Grounded m_Grounded;
	private float m_ClimbDirection;

	public void Mount()
	{
		if (m_Grounded.OnGround)
		{
			return;
		}

		if (Mounted)
		{
			return;
		}

		var furthestLink = GetFurthestMountableLink();

		if (furthestLink == null)
		{
			return;
		}

		(m_Chain, m_CurrentLinkIndex) = furthestLink;

		var link = m_Chain.Links[m_CurrentLinkIndex];

		m_Attachment = gameObject.AddComponent<DistanceJoint2D>();
		m_Attachment.connectedBody = link;
		m_Attachment.autoConfigureConnectedAnchor = false;
		m_Attachment.connectedAnchor = Vector2.zero;
		m_Attachment.anchor = Vector2.zero;
		m_Attachment.autoConfigureDistance = false;
		//m_Attachment.dampingRatio = 1;
		//m_Attachment.frequency = 50;
		//m_Attachment.maxDistanceOnly = true;
		m_Attachment.distance = 0.5f;
	}

	public void Dismount()
	{
		if (!Mounted)
		{
			return;
		}

		Destroy(m_Attachment);
		m_Attachment = null;

		if (!m_Grounded.OnGround)
		{
			m_RigidBody.velocity = Quaternion.Euler(0, 0, DismountMaxAngle * -m_DismountDirection) * Vector2.up * DismountVelocity;
		}
	}

	public void Climb(float direction)
	{
		m_ClimbDirection = direction;
	}

	private void OnMove(InputValue value)
	{
		m_DismountDirection = value.Get<float>();
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

	private void FixedUpdate()
	{
		if (!Mounted)
		{
			return;
		}

		if (m_Grounded.OnGround)
		{
			Dismount();
			return;
		}

		//if (!Mathf.Approximately(m_ClimbDirection, 0))
		//{
		//    var velocity = -m_Attachment.connectedBody.transform.up * m_ClimbDirection * ClimbSpeed * Time.fixedDeltaTime;
		//    m_Rigidbody.MovePosition(transform.position + velocity);
		//}
		//var force = -m_Attachment.connectedBody.transform.up * m_ClimbDirection * 80;
		//m_Rigidbody.AddForce(force);

		//var anchorWorldPosition = m_Attachment.attachedRigidbody.transform.TransformPoint(m_Attachment.connectedAnchor);
		//var distance = Vector2.Distance(transform.position, anchorWorldPosition);

		m_Attachment.connectedAnchor -= new Vector2(0, m_ClimbDirection * ClimbSpeed * Time.fixedDeltaTime);

		HandleLinkTransitions();

		var anchorOffset = m_Attachment.connectedAnchor.y;
		anchorOffset = Mathf.Clamp(anchorOffset, -m_Chain.LinkAnchorOffset, m_Chain.LinkAnchorOffset);
		m_Attachment.connectedAnchor = new Vector2(0, anchorOffset);
	}

	private void HandleLinkTransitions()
	{
		if (m_Attachment.connectedAnchor.y > m_Chain.LinkAnchorOffset && m_CurrentLinkIndex < m_Chain.Links.Length - 1)
		{
			var nextLink = m_Chain.Links[++m_CurrentLinkIndex];

			m_Attachment.connectedBody = nextLink;
			m_Attachment.connectedAnchor = new Vector2(0, -m_Chain.LinkAnchorOffset);
		}
		else if (m_Attachment.connectedAnchor.y < -m_Chain.LinkAnchorOffset && m_CurrentLinkIndex > 1)
		{
			var prevLink = m_Chain.Links[--m_CurrentLinkIndex];

			m_Attachment.connectedBody = prevLink;
			m_Attachment.connectedAnchor = new Vector2(0, m_Chain.LinkAnchorOffset);
		}
	}

	private IEnumerable<MountableLink> GetFurthestMountableLinks()
	{
		var linkFilter = new ContactFilter2D();
		linkFilter.layerMask = LayerMask.GetMask("Chain");

		var linksCount = Physics2D.OverlapCircle(transform.position, MountRadius, linkFilter, m_MountableLinks);

		// Group links by their chain and return the chains with their furthest link index
		return m_MountableLinks.Take(linksCount)
			.Where(x => x.GetComponentInParent<Chain>() != null)
			.Select(x => new MountableLink { Chain = x.GetComponentInParent<Chain>(), LinkIndex = x.transform.GetSiblingIndex() })
			.GroupBy(x => x.Chain)
			.Select(x => new MountableLink { Chain = x.Key, LinkIndex = x.Min(y => y.LinkIndex) });
	}

	private MountableLink GetFurthestMountableLink()
	{
		var chains = GetFurthestMountableLinks();

		var furthestChain = chains
			.OrderByDescending(x => x.Chain.MountPriority)
			.FirstOrDefault();

		return furthestChain;
	}

	private void Awake()
	{
		m_RigidBody = GetComponent<Rigidbody2D>();
		m_Grounded = GetComponent<Grounded>();
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			return;
		}

		if (Mounted)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(m_Attachment.connectedBody.transform.TransformPoint(m_Attachment.connectedAnchor), 0.15f);

			var climbDirection = new Ray(transform.position, -m_Attachment.connectedBody.transform.up * m_ClimbDirection);
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(climbDirection);
		}
		else
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(transform.position, MountRadius);

			var furthestLink = GetFurthestMountableLink();

			if (furthestLink != null)
			{
				var (chain, linkIndex) = furthestLink;
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(chain.Links[linkIndex].transform.position, 0.15f);
			}
		}
	}
}
