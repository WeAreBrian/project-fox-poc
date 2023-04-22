using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ColliderCorners
{
	private static readonly Dictionary<Type, Func<Collider2D, Vector2, ColliderCorner>> s_GetCorners = new Dictionary<Type, Func<Collider2D, Vector2, ColliderCorner>>
	{
		{ typeof(BoxCollider2D), (collider, point) => GetCorner((BoxCollider2D)collider, point) },
		{ typeof(CircleCollider2D), (collider, point) => GetCorner((CircleCollider2D)collider, point) },
		{ typeof(PolygonCollider2D), (collider, point) => GetCorner((PolygonCollider2D)collider, point) },
	};

	public static ColliderCorner GetCorner(Collider2D collider, Vector2 point)
	{
		if (s_GetCorners.TryGetValue(collider.GetType(), out var getCorner))
		{
			return getCorner(collider, point);
		}
		else
		{
			throw new ArgumentException($"{collider.GetType().Name} for chain is not supported yet.");
		}
	}

	private static ColliderCorner GetCorner(CircleCollider2D collider, Vector2 point)
	{
		return new ColliderCorner
		{
			Position = point,
			Normal = (point - (Vector2)collider.transform.TransformPoint(collider.offset)).normalized
		};
	}

	private static ColliderCorner GetCorner(BoxCollider2D collider, Vector2 point)
	{
		var corners = new Vector2[]
		{
			collider.transform.TransformPoint(collider.offset + collider.size * new Vector2(-0.5f, -0.5f)),
			collider.transform.TransformPoint(collider.offset + collider.size * new Vector2(-0.5f, 0.5f)),
			collider.transform.TransformPoint(collider.offset + collider.size * new Vector2(0.5f, 0.5f)),
			collider.transform.TransformPoint(collider.offset + collider.size * new Vector2(0.5f, -0.5f))
		}.Select(x => new ColliderCorner { Position = x, Normal = (x - (Vector2)collider.transform.TransformPoint(collider.offset)).normalized });

		return corners.OrderBy(x => Vector2.Distance(point, x.Position)).First();
	}

	private static ColliderCorner GetCorner(PolygonCollider2D collider, Vector2 point)
	{
		return new ColliderCorner();
	}
}
