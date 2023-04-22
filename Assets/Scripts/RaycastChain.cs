using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RaycastChain : MonoBehaviour
{
	public struct Corner
	{
		public Vector2 Position;
		public Vector2 Normal;
	};

	public float Thickness = 0.1f;
	public float MaxLength = 15;
	public LayerMask CollisionMask;
	public Rigidbody2D Anchor;
	public Rigidbody2D Player;

	private const int k_RaycastSteps = 16;
	private const float k_MinCornerDistance = 0.001f;

	private static readonly Dictionary<Type, Func<Collider2D, Vector2, Corner>> s_GetCorners = new Dictionary<Type, Func<Collider2D, Vector2, Corner>>
	{
		{ typeof(BoxCollider2D), (collider, point) => GetCorner((BoxCollider2D)collider, point) },
		{ typeof(CircleCollider2D), (collider, point) => GetCorner((CircleCollider2D)collider, point) },
		{ typeof(PolygonCollider2D), (collider, point) => GetCorner((PolygonCollider2D)collider, point) },
	};

	private LineRenderer m_LineRenderer;
	private List<Corner> m_Corners = new List<Corner>();
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
		UpdateLine();

		if (NewCorner(m_Corners.Count > 0 ? GetPosition(m_Corners.Last()) : Anchor.position, Player.position, m_PreviousPlayerPosition, out var endCorner))
		{
			m_Corners.Add(endCorner);
		}

		if (NewCorner(m_Corners.Count > 0 ? GetPosition(m_Corners.First()) : Player.position, Anchor.position, m_PreviousAnchorPosition, out var startCorner))
		{
			m_Corners.Insert(0, startCorner);
		}

		m_PreviousAnchorPosition = Anchor.position;
		m_PreviousPlayerPosition = Player.position;

		if (m_Corners.Count == 0)
		{
			return;
		}

		if (CanRemoveCorner(m_Corners.Last(), m_Corners.Count > 1 ? GetPosition(m_Corners[m_Corners.Count - 2]) : Anchor.position, Player.position))
		{
			m_Corners.RemoveAt(m_Corners.Count - 1);
		}

		if (m_Corners.Count == 0)
		{
			return;
		}

		if (CanRemoveCorner(m_Corners.First(), m_Corners.Count > 1 ? GetPosition(m_Corners[1]) : Player.position, Anchor.position))
		{
			m_Corners.RemoveAt(0);
		}
	}

	private bool NewCorner(Vector2 from, Vector2 to, Vector2 previousTo, out Corner corner)
	{
		corner = new Corner();

		if (Physics2D.OverlapPoint(to, CollisionMask) != null)
		{
			return false;
		}

		var hit = LineCastSweep(from, to, previousTo);

		if (!hit)
		{
			return false;
		}

		corner = GetCorner(hit.collider, hit.point);

		if (Vector2.Distance(GetPosition(corner), from) < k_MinCornerDistance)
		{
			return false;
		}

		return true;
	}
	
	private bool CanRemoveCorner(Corner corner, Vector2 previousPosition, Vector2 nextPosition)
	{
		var previousEdgeDirection = (GetPosition(corner) - previousPosition).normalized;
		var previousEdgeNormal = Vector2.Perpendicular(previousEdgeDirection);

		var clockwiseCorner = Vector2.Dot(previousEdgeNormal, corner.Normal) < 0;

		if (clockwiseCorner)
		{
			previousEdgeNormal *= -1;
		}

		var directionToNext = (nextPosition - GetPosition(corner)).normalized;

		return Vector2.Dot(previousEdgeNormal, directionToNext) > 0;
	}

	private Vector2 GetPosition(Corner corner)
	{
		return corner.Position + corner.Normal * Thickness / 2;
	}

	private void UpdateLine()
	{
		m_LineRenderer.positionCount = m_Corners.Count + 2;
		m_LineRenderer.SetPosition(0, Anchor.position);

		for (var i = 0; i < m_Corners.Count; i++)
		{
			m_LineRenderer.SetPosition(i + 1, GetPosition(m_Corners[i]));
		}

		m_LineRenderer.SetPosition(m_Corners.Count + 1, Player.position);
	}

	private RaycastHit2D LineCastSweep(Vector2 origin, Vector2 from, Vector2 to)
	{
		for (var i = 0; i <= k_RaycastSteps; i++)
		{
			var end = Vector2.Lerp(to, from, i / (float)k_RaycastSteps);
			var hit = Physics2D.Linecast(origin, end, CollisionMask);

			if (hit)
			{
				return hit;
			}
		}

		return new RaycastHit2D();
	}

	private static Corner GetCorner(Collider2D collider, Vector2 point)
	{
		if (s_GetCorners.TryGetValue(collider.GetType(), out var corners))
		{
			return corners(collider, point);
		}
		else
		{
			throw new ArgumentException($"{collider.GetType().Name} for chain is not supported yet.");
		}
	}

	private static Corner GetCorner(CircleCollider2D collider, Vector2 point)
	{
		return new Corner
		{
			Position = point,
			Normal = (point - (Vector2)collider.transform.TransformPoint(collider.offset)).normalized
		};
	}

	private static Corner GetCorner(BoxCollider2D collider, Vector2 point)
	{
		var corners = new Vector2[]
		{
			(Vector2)collider.transform.TransformPoint(collider.offset + collider.size * new Vector2(-0.5f, -0.5f)),
			(Vector2)collider.transform.TransformPoint(collider.offset + collider.size * new Vector2(-0.5f, 0.5f)),
			(Vector2)collider.transform.TransformPoint(collider.offset + collider.size * new Vector2(0.5f, 0.5f)),
			(Vector2)collider.transform.TransformPoint(collider.offset + collider.size * new Vector2(0.5f, -0.5f))
		}.Select(x => new Corner { Position = x, Normal = (x - (Vector2)collider.transform.TransformPoint(collider.offset)).normalized });

		return corners.OrderBy(x => Vector2.Distance(point, x.Position)).First();
	}

	private static Corner GetCorner(PolygonCollider2D collider, Vector2 point)
	{
		return new Corner();
	}
}
