using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RaycastChain : MonoBehaviour
{
	public float Thickness = 0.1f;
	public float MaxLength = 15;
	public LayerMask CollisionMask;
	public Rigidbody2D Anchor;
	public Rigidbody2D Player;

	private const int k_SweepSteps = 16;
	private const float k_MinCornerDistance = 0.001f;

	private LineRenderer m_LineRenderer;
	private List<ChainCorner> m_Corners = new List<ChainCorner>();
	private Vector2 m_PreviousAnchorPosition;
	private Vector2 m_PreviousPlayerPosition;

	private void Awake()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
	}

	private void Start()
	{
		m_LineRenderer.startWidth = Thickness;
		m_LineRenderer.endWidth = Thickness;

		m_PreviousAnchorPosition = Anchor.position;
		m_PreviousPlayerPosition = Player.position;
	}

	private void FixedUpdate()
	{
		UpdateCorners();
		UpdateDistanceJoints();
		UpdateLineRenderer();
		ApplyTensionForces();

		m_PreviousAnchorPosition = Anchor.position;
		m_PreviousPlayerPosition = Player.position;
	}

	private bool NewCorner(Vector2 from, Vector2 to, Vector2 previousTo, out ChainCorner corner)
	{
		corner = new ChainCorner();

		if (Physics2D.OverlapPoint(to, CollisionMask) != null)
		{
			return false;
		}

		var hit = LineCastSweep(from, to, previousTo);

		if (!hit)
		{
			return false;
		}

		var colliderCorner = ColliderCorners.GetCorner(hit.collider, hit.point);
		corner.Position = colliderCorner.Position + colliderCorner.Normal * Thickness / 2;
		corner.Normal = colliderCorner.Normal;

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

		if (m_Corners.Count == 0)
		{
			return;
		}

		if (CanRemoveCorner(m_Corners.Last(), m_Corners.Count > 1 ? m_Corners[m_Corners.Count - 2].Position : Anchor.position, Player.position))
		{
			m_Corners.RemoveAt(m_Corners.Count - 1);
		}

		if (m_Corners.Count == 0)
		{
			return;
		}

		if (CanRemoveCorner(m_Corners.First(), m_Corners.Count > 1 ? m_Corners[1].Position : Player.position, Anchor.position))
		{
			m_Corners.RemoveAt(0);
		}
	}

	private void UpdateDistanceJoints()
	{

	}

	private void ApplyTensionForces()
	{

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
			var hit = Physics2D.Linecast(origin, end, CollisionMask);

			if (hit)
			{
				return hit;
			}
		}

		return new RaycastHit2D();
	}
}
