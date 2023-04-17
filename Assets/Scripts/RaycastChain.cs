using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class RaycastChain : MonoBehaviour
{
	public struct Corner
	{
		public Vector2 Position;
		public Vector2 Normal;
	};

	public float Thickness = 0.1f;
	public Transform From;
	public Transform To;

	public Vector2 FromPosition => new Vector2(From.position.x, From.position.y);
	public Vector2 ToPosition => new Vector2(To.position.x, To.position.y);

	private static readonly Dictionary<Type, Func<Collider2D, Vector2, Corner>> s_GetCorners = new Dictionary<Type, Func<Collider2D, Vector2, Corner>>
	{
		{ typeof(BoxCollider2D), (collider, point) => GetClosestCorner((BoxCollider2D)collider, point) },
		{ typeof(CircleCollider2D), (collider, point) => GetClosestCorner((CircleCollider2D)collider, point) },
		{ typeof(PolygonCollider2D), (collider, point) => GetClosestCorner((PolygonCollider2D)collider, point) },
	};

	private LineRenderer m_LineRenderer;
	private List<Corner> m_Corners = new List<Corner>();

	private void Awake()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
	}

	private void Start()
	{
		m_LineRenderer.startWidth = Thickness;
		m_LineRenderer.endWidth = Thickness;
	}

	private void Update()
	{
		UpdateLine();
		AddCorners();
		RemoveCorners();
	}

	private void AddCorners()
	{
		var from = m_Corners.Count > 0 ? GetPosition(m_Corners.Last()) : FromPosition;
		var hit = Physics2D.Raycast(from, (ToPosition - from).normalized, Vector2.Distance(from, ToPosition));

		if (!hit)
		{
			return;
		}

		var corner = s_GetCorners[hit.collider.GetType()](hit.collider, hit.point);

		if (m_Corners.Count > 0 && m_Corners.Last().Position == corner.Position)
		{
			return;
		}

		m_Corners.Add(corner);
	}

	private void RemoveCorners()
	{
		if (m_Corners.Count <= 0)
		{
			return;
		}

		var corner = m_Corners[m_Corners.Count - 1];
		var previousCornerPos = m_Corners.Count > 1 ? GetPosition(m_Corners[m_Corners.Count - 2]) : (Vector2)transform.position;
		var direction = (GetPosition(corner) - previousCornerPos).normalized;
		var clockwise = Vector2.SignedAngle(corner.Normal, direction) < 0;

		var lineNormal = (clockwise ? 1 : -1) * Vector2.Perpendicular(direction);

		var toDirection = (ToPosition - GetPosition(corner)).normalized;

		if (Vector2.Dot(toDirection, lineNormal) > 0)
		{
			m_Corners.RemoveAt(m_Corners.Count - 1);
		}
	}

	private Vector2 GetPosition(Corner corner)
	{
		return corner.Position + corner.Normal * Thickness / 2;
	}

	private void UpdateLine()
	{
		m_LineRenderer.positionCount = m_Corners.Count + 2;
		m_LineRenderer.SetPosition(0, From.position);

		for (var i = 0; i < m_Corners.Count; i++)
		{
			m_LineRenderer.SetPosition(i + 1, GetPosition(m_Corners[i]));
		}

		m_LineRenderer.SetPosition(m_Corners.Count + 1, To.position);
	}

	private void OnDrawGizmos()
	{
		foreach (var corner in m_Corners)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(GetPosition(corner), 0.05f);
		}

		var from = m_Corners.Count > 0 ? GetPosition(m_Corners.Last()) : FromPosition;
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(from, ToPosition);
	}

	private static Corner GetClosestCorner(CircleCollider2D collider, Vector2 point)
	{
		return new Corner
		{
			Position = point,
			Normal = (point - (Vector2)collider.transform.position).normalized
		};
	}

	private static Corner GetClosestCorner(BoxCollider2D collider, Vector2 point)
	{
		return GetCorners(collider).OrderBy(x => Vector2.Distance(point, x.Position)).First();
	}

	private static IEnumerable<Corner> GetCorners(BoxCollider2D collider)
	{
		return new[]
		{
			(Vector2)collider.transform.TransformPoint(collider.offset + new Vector2(-collider.size.x, -collider.size.y) / 2),
			(Vector2)collider.transform.TransformPoint(collider.offset + new Vector2(-collider.size.x, collider.size.y) / 2),
			(Vector2)collider.transform.TransformPoint(collider.offset + new Vector2(collider.size.x, collider.size.y) / 2),
			(Vector2)collider.transform.TransformPoint(collider.offset + new Vector2(collider.size.x, -collider.size.y) / 2)
		}.Select(x => new Corner { Position = x, Normal = (x - (Vector2)collider.transform.position).normalized });
	}

	private static Corner GetClosestCorner(PolygonCollider2D collider, Vector2 point)
	{
		return new Corner();
	}
}
