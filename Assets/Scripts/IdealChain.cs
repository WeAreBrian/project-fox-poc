using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class IdealChain : MonoBehaviour
{
	public UnityEvent<ChainPoint> PointAdded;
	public UnityEvent<ChainPoint> PointRemoved;
	public LayerMask Collision;
	public float Width = 0.15f;
	public float MaxLength = 15;
	public float MaxTensionForce = 1000;
	public Rigidbody2D Anchor;
	public Rigidbody2D Player;

	public bool HasPendulumPoints => m_Points.Count > 2;
	public Vector2 AnchorPendulumPoint => m_Points[1].Position;
	public Vector2 PlayerPendulumPoint => m_Points[m_Points.Count - 2].Position;
	public Rigidbody2D AnchorPendulum => HasPendulumPoints ? m_AnchorDistanceJoint.attachedRigidbody : Player;
	public Rigidbody2D PlayerPendulum => HasPendulumPoints ? m_PlayerDistanceJoint.attachedRigidbody : Anchor;
	public Vector2 AnchorToPendulum => (AnchorPendulumPoint - Anchor.position).normalized;
	public Vector2 PlayerToPendulum => (PlayerPendulumPoint - Player.position).normalized;
	public float AnchorTension => m_AnchorDistanceJoint.reactionForce.magnitude;
	public float PlayerTension => m_PlayerDistanceJoint.reactionForce.magnitude;
	public float Length => GetLength();

	private const int k_SweepSteps = 16;
	private const float k_MinPointDistance = 0.01f;

	private LineRenderer m_LineRenderer;
	private List<ChainPoint> m_Points;
	private DistanceJoint2D m_AnchorDistanceJoint;
	private DistanceJoint2D m_PlayerDistanceJoint;
	private DistanceJoint2D m_MaxDistanceJoint;

	private void Awake()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
	}

	private void Start()
	{
		m_LineRenderer.startWidth = Width;
		m_LineRenderer.endWidth = Width;

		m_Points = new List<ChainPoint>
		{
			new ChainPoint(Anchor),
			new ChainPoint(Player),
		};

		m_AnchorDistanceJoint = CreatePendulum(Anchor);
		m_PlayerDistanceJoint = CreatePendulum(Player);

		m_MaxDistanceJoint = Player.gameObject.AddComponent<DistanceJoint2D>();
		m_MaxDistanceJoint.autoConfigureConnectedAnchor = false;
		m_MaxDistanceJoint.autoConfigureDistance = false;
		m_MaxDistanceJoint.maxDistanceOnly = true;
		m_MaxDistanceJoint.anchor = Vector2.zero;
		m_MaxDistanceJoint.connectedAnchor = Vector2.zero;
		m_MaxDistanceJoint.connectedBody = Anchor;
		m_MaxDistanceJoint.distance = MaxLength;
		m_MaxDistanceJoint.enableCollision = true;

		AnchorHolder.pickup += ResetPoints;
	}

	private void ResetPoints()
	{
		m_Points.Clear();

		m_Points = new List<ChainPoint>
		{
			new ChainPoint(Anchor),
			new ChainPoint(Player),
		};
	}

	private void FixedUpdate()
	{
		UpdatePoints();
		UpdateDistanceJoints();
		ApplyTensionForces();
		UpdateLineRenderer();
	}

	private bool Sweep(Vector2 origin, Vector2 from, Vector2 to, out ChainPoint point)
	{
		point = null;

		if (Physics2D.OverlapPoint(from, Collision) != null)
		{
			return false;
		}

		var hit = LineCastSweep(origin, from, to);

		if (!hit)
		{
			return false;
		}

		var colliderCorner = ColliderCorners.GetCorner(hit.collider, hit.point);
		var offset = hit.transform.InverseTransformPoint(colliderCorner.Position + colliderCorner.Normal * Width / 2);
		var corner = hit.transform.InverseTransformPoint(colliderCorner.Position);

		point = new ChainPoint(hit.collider, offset, corner);

		if (Vector2.Distance(point.Position, origin) < k_MinPointDistance)
		{
			return false;
		}

		return true;
	}

	private Vector2 GetLineNormal(ChainPoint point, ChainPoint previousPoint)
	{
		var previousToPoint = (point.Position - previousPoint.Position).normalized;
		var lineNormal = Vector2.Perpendicular(previousToPoint);
		var clockwisePoint = Vector2.Dot(lineNormal, point.Normal) < 0;

		return clockwisePoint ? -lineNormal : lineNormal;
	}

	private bool IsRemovable(ChainPoint point, ChainPoint previousPoint, ChainPoint nextPoint)
	{
		var pointToNext = (nextPoint.Position - point.Position).normalized;

		return Vector2.Dot(GetLineNormal(point, previousPoint), pointToNext) > 0;
	}

	private void UpdatePoints()
	{
		AddPoints();
		RemovePoints();

		foreach (var point in m_Points)
		{
			point.OldPosition = point.Position;
		}
	}

	private void AddPoints()
	{
		for (var i = 0; i < m_Points.Count - 1; i++)
		{
			var point = m_Points[i];
			var nextPoint = m_Points[i + 1];

			if (Sweep(point.Position, nextPoint.OldPosition, nextPoint.Position, out var newPoint))
			{
				m_Points.Insert(++i, newPoint);
				PointAdded?.Invoke(newPoint);
			}
		}
	}

	private void RemovePoints()
	{
		for (var i = 1; i < m_Points.Count - 1; i++)
		{
			var previousPoint = m_Points[i - 1];
			var point = m_Points[i];
			var nextPoint = m_Points[i + 1];

			if (IsRemovable(point, previousPoint, nextPoint))
			{
				m_Points.RemoveAt(i--);
				PointRemoved?.Invoke(point);
			}
		}
	}

	private void UpdateDistanceJoints()
	{
		m_MaxDistanceJoint.distance = MaxLength;

		m_PlayerDistanceJoint.enabled = HasPendulumPoints;
		m_AnchorDistanceJoint.enabled = HasPendulumPoints;

		if (!HasPendulumPoints)
		{
			return;
		}

		m_PlayerDistanceJoint.transform.position = PlayerPendulumPoint;
		m_PlayerDistanceJoint.distance = Mathf.Max(0, MaxLength - (Length - Vector2.Distance(Player.position, PlayerPendulumPoint)));

		m_AnchorDistanceJoint.transform.position = AnchorPendulumPoint;
		m_AnchorDistanceJoint.distance = Mathf.Max(0, MaxLength - (Length - Vector2.Distance(Anchor.position, AnchorPendulumPoint)));
	}

	private DistanceJoint2D CreatePendulum(Rigidbody2D connectedBody)
	{
		var pendulum = new GameObject($"{connectedBody.name}Pendulum", typeof(Rigidbody2D), typeof(DistanceJoint2D));
		pendulum.transform.parent = transform;
		pendulum.transform.position = Vector2.zero;

		var rigidBody = pendulum.GetComponent<Rigidbody2D>();
		rigidBody.bodyType = RigidbodyType2D.Static;

		var distanceJoint = pendulum.GetComponent<DistanceJoint2D>();
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
		if (!HasPendulumPoints)
		{
			return;
		}

		if (Anchor.bodyType != RigidbodyType2D.Dynamic)
		{
			return;
		}

		var forceOnAnchor = Mathf.Min(MaxTensionForce, PlayerTension);
		Anchor.AddForce(AnchorToPendulum * forceOnAnchor);

		var forceOnPlayer = Mathf.Min(MaxTensionForce, AnchorTension);
		Player.AddForce(PlayerToPendulum * forceOnPlayer);
	}

	private void UpdateLineRenderer()
	{
		if (m_LineRenderer == null)
		{
			return;
		}

		m_LineRenderer.positionCount = m_Points.Count;

		for (var i = 0; i < m_Points.Count; i++)
		{
			m_LineRenderer.SetPosition(i, m_Points[i].Position);
		}
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

	private float GetLength()
	{
		var length = 0f;

		for (var i = 0; i < m_Points.Count - 1; i++)
		{
			var point = m_Points[i];
			var nextPoint = m_Points[i + 1];

			length += Vector2.Distance(point.Position, nextPoint.Position);
		}

		return length;
	}

	private void OnDrawGizmosSelected()
	{
		if (m_Points == null)
		{
			return;
		}
		
		for (var i = 0; i < m_Points.Count; i++)
		{
			var point = m_Points[i];

			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(point.Position, Width / 2);

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(point.Position, point.Normal);
		}

		Gizmos.color = Color.blue;

		for (var i = 0; i < m_Points.Count - 1; i++)
		{
			var point = m_Points[i];
			var nextPoint = m_Points[i + 1];

			Gizmos.DrawLine(point.Position, nextPoint.Position);
		}

		Gizmos.color = Color.red;

		for (var i = 1; i < m_Points.Count; i++)
		{
			var previousPoint = m_Points[i - 1];
			var point = m_Points[i];

			var lineNormal = GetLineNormal(point, previousPoint);

			Gizmos.DrawRay((previousPoint.Position + point.Position) / 2, lineNormal);
		}
	}
}
