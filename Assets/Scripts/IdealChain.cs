using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IdealChain : MonoBehaviour
{
	public LayerMask Collision;
	public float Width = 0.15f;
	public float MaxLength = 15;
	public float MaxTensionForce = 1000;
	public Rigidbody2D Anchor;
	public Rigidbody2D Player;

	private const int k_SweepSteps = 16;
	private const float k_MinCornerDistance = 0.001f;

	private LineRenderer m_LineRenderer;
	private List<ChainCorner> m_Corners = new List<ChainCorner>();
	private Vector2 m_PreviousAnchorPosition;
	private Vector2 m_PreviousPlayerPosition;
	private DistanceJoint2D m_AnchorDistanceJoint;
	private DistanceJoint2D m_PlayerDistanceJoint;
	private DistanceJoint2D m_MaxDistanceJoint;

	public float GetLength()
	{
		if (m_Corners.Count == 0)
		{
			return Vector2.Distance(Anchor.position, Player.position);
		}

		var length = 0.0f;

		length += Vector2.Distance(Anchor.position, m_Corners.First().Position);
		length += Vector2.Distance(Player.position, m_Corners.Last().Position);

		for (var i = 0; i < m_Corners.Count - 1; i++)
		{
			var corner = m_Corners[i];
			var nextCorner = m_Corners[i + 1];

			length += Vector2.Distance(corner.Position, nextCorner.Position);
		}

		return length;
	}

	public float GetSegmentLength()
	{
		if (m_Corners.Count == 0)
		{
			return 0;
		}

		var length = 0.0f;

		for (var i = 0; i < m_Corners.Count - 1; i++)
		{
			var corner = m_Corners[i];
			var nextCorner = m_Corners[i + 1];

			length += Vector2.Distance(corner.Position, nextCorner.Position);
		}

		return length;
	}

	private void Awake()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
	}

	private void Start()
	{
		m_LineRenderer.startWidth = Width;
		m_LineRenderer.endWidth = Width;

		m_PreviousAnchorPosition = Anchor.position;
		m_PreviousPlayerPosition = Player.position;

		m_AnchorDistanceJoint = CreateDistanceJoint(Anchor);
		m_PlayerDistanceJoint = CreateDistanceJoint(Player);

		m_MaxDistanceJoint = Player.gameObject.AddComponent<DistanceJoint2D>();
		m_MaxDistanceJoint.autoConfigureConnectedAnchor = false;
		m_MaxDistanceJoint.autoConfigureDistance = false;
		m_MaxDistanceJoint.maxDistanceOnly = true;
		m_MaxDistanceJoint.anchor = Vector2.zero;
		m_MaxDistanceJoint.connectedAnchor = Vector2.zero;
		m_MaxDistanceJoint.connectedBody = Anchor;
		m_MaxDistanceJoint.distance = MaxLength;
	}

	private void FixedUpdate()
	{
		UpdateCorners();
		UpdateDistanceJoints();
		ApplyTensionForces();
		UpdateLineRenderer();

		m_PreviousAnchorPosition = Anchor.position;
		m_PreviousPlayerPosition = Player.position;
	}

	private bool NewCorner(Vector2 from, Vector2 to, Vector2 previousTo, out ChainCorner corner)
	{
		corner = new ChainCorner();

		if (Physics2D.OverlapPoint(to, Collision) != null)
		{
			return false;
		}

		var hit = LineCastSweep(from, to, previousTo);

		if (!hit)
		{
			return false;
		}

		var colliderCorner = ColliderCorners.GetCorner(hit.collider, hit.point);

		corner.Position = colliderCorner.Position + colliderCorner.Normal * Width / 2;
		corner.Normal = colliderCorner.Normal;
		corner.Collider = hit.collider;

		if (Vector2.Distance(corner.Position, from) < k_MinCornerDistance)
		{
			return false;
		}

		return true;
	}
	
	private bool CanRemoveCorner(ChainCorner corner, Vector2 previousPosition, Vector2 nextPosition)
	{
		var previousEdgeDirection = (corner.Position - previousPosition).normalized;
		var previousEdgeNormal = Vector2.Perpendicular(previousEdgeDirection);

		var clockwiseCorner = Vector2.Dot(previousEdgeNormal, corner.Normal) < 0;

		if (clockwiseCorner)
		{
			previousEdgeNormal *= -1;
		}

		var directionToNext = (nextPosition - corner.Position).normalized;

		return Vector2.Dot(previousEdgeNormal, directionToNext) > 0;
	}

	private void UpdateCorners()
	{
		if (NewCorner(m_Corners.Count > 0 ? m_Corners.Last().Position : Anchor.position, Player.position, m_PreviousPlayerPosition, out var endCorner))
		{
			m_Corners.Add(endCorner);
		}

		if (NewCorner(m_Corners.Count > 0 ? m_Corners.First().Position : Player.position, Anchor.position, m_PreviousAnchorPosition, out var startCorner))
		{
			m_Corners.Insert(0, startCorner);
		}

		if (m_Corners.Count != 0 && CanRemoveCorner(m_Corners.Last(), m_Corners.Count > 1 ? m_Corners[m_Corners.Count - 2].Position : Anchor.position, Player.position))
		{
			m_Corners.RemoveAt(m_Corners.Count - 1);
		}

		if (m_Corners.Count != 0 && CanRemoveCorner(m_Corners.First(), m_Corners.Count > 1 ? m_Corners[1].Position : Player.position, Anchor.position))
		{
			m_Corners.RemoveAt(0);
		}
	}

	private void UpdateDistanceJoints()
	{
		m_MaxDistanceJoint.distance = MaxLength;

		if (m_Corners.Count == 0)
		{
			m_PlayerDistanceJoint.enabled = false;
			m_AnchorDistanceJoint.enabled = false;
			return;
		}

		m_PlayerDistanceJoint.enabled = true;
		m_PlayerDistanceJoint.transform.position = m_Corners.Last().Position;
		m_PlayerDistanceJoint.distance = Mathf.Max(0, MaxLength - (Vector2.Distance(Anchor.position, m_Corners.First().Position) + GetSegmentLength()));

		m_AnchorDistanceJoint.enabled = true;
		m_AnchorDistanceJoint.transform.position = m_Corners.First().Position;
		m_AnchorDistanceJoint.distance = Mathf.Max(0, MaxLength - (Vector2.Distance(Player.position, m_Corners.Last().Position) + GetSegmentLength()));
	}

	private DistanceJoint2D CreateDistanceJoint(Rigidbody2D connectedBody)
	{
		var corner = new GameObject($"{connectedBody.name}DistanceJoint", typeof(Rigidbody2D), typeof(DistanceJoint2D));
		corner.transform.parent = transform;
		corner.transform.position = Vector2.zero;

		var rigidBody = corner.GetComponent<Rigidbody2D>();
		rigidBody.bodyType = RigidbodyType2D.Static;

		var distanceJoint = corner.GetComponent<DistanceJoint2D>();
		distanceJoint.autoConfigureConnectedAnchor = false;
		distanceJoint.autoConfigureDistance = false;
		distanceJoint.maxDistanceOnly = true;
		distanceJoint.anchor = Vector2.zero;
		distanceJoint.connectedAnchor = Vector2.zero;
		distanceJoint.connectedBody = connectedBody;
		distanceJoint.distance = MaxLength;
		distanceJoint.enabled = false;

		return distanceJoint;
	}

	private void ApplyTensionForces()
	{
		if (m_Corners.Count == 0)
		{
			return;
		}

		var forceOnAnchor = Mathf.Min(MaxTensionForce, m_PlayerDistanceJoint.reactionForce.magnitude);
		Anchor.AddForce((m_Corners.First().Position - Anchor.position).normalized * forceOnAnchor);

		var forceOnPlayer = Mathf.Min(MaxTensionForce, m_AnchorDistanceJoint.reactionForce.magnitude);
		Player.AddForce((m_Corners.Last().Position - Player.position).normalized * forceOnPlayer);
	}

	private void UpdateLineRenderer()
	{
		m_LineRenderer.positionCount = m_Corners.Count + 2;
		m_LineRenderer.SetPosition(0, Anchor.position);

		for (var i = 0; i < m_Corners.Count; i++)
		{
			m_LineRenderer.SetPosition(i + 1, m_Corners[i].Position);
		}

		m_LineRenderer.SetPosition(m_Corners.Count + 1, Player.position);
	}

	private RaycastHit2D LineCastSweep(Vector2 origin, Vector2 from, Vector2 to)
	{
		for (var i = 0; i < k_SweepSteps; i++)
		{
			var end = Vector2.Lerp(to, from, (i + 1) / (float)k_SweepSteps);
			var hit = Physics2D.Linecast(origin, end, Collision);

			if (hit)
			{
				return hit;
			}
		}

		return new RaycastHit2D();
	}

	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < m_Corners.Count; i++)
		{
			var corner = m_Corners[i];

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(corner.Position, Width / 2);

			Gizmos.color = Color.red;
			Gizmos.DrawRay(corner.Position, corner.Normal);

			if (i < m_Corners.Count - 1)
			{
				var nextCorner = m_Corners[i + 1];

				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(corner.Position, nextCorner.Position);
			}
		}
	}
}
