using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainPoint
{
    public Vector2 Offset;
    public Vector2 Normal;
    public Vector2 OldPosition;
    public Collider2D Collider;
    public Rigidbody2D Rigidbody;

    public Vector2 Position => Rigidbody.transform.TransformPoint(Offset);//Rigidbody.GetPoint(Offset);
}
