using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainPoint
{
    public Vector2 Offset;
    public Vector2 Corner;
    public Vector2 OldPosition;
    public Collider2D Collider;
    public Rigidbody2D Rigidbody;

    public Vector2 Position => Rigidbody.transform.TransformPoint(Offset);
    public Vector2 Normal => (Position - (Vector2)Rigidbody.transform.TransformPoint(Corner)).normalized;

	public ChainPoint(Rigidbody2D rigidbody)
    {
        Rigidbody = rigidbody;
        OldPosition = Position;
    }

	public ChainPoint(Collider2D collider, Vector2 offset, Vector2 corner)
	{
        Collider = collider;
        Rigidbody = collider.attachedRigidbody;
        Offset = offset;
        Corner = corner;
        OldPosition = Position;
	}
}
