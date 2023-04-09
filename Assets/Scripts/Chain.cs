using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Chain : MonoBehaviour
{
	public event Action LinksCreated;
	public Vector2 LinkSize = new Vector2(0.3f, 0.9f);
	public float LinkMass = 0.1f;
	public Sprite LinkSprite;
	public Transform From;
	public Transform To;
	public float MaxLength = 20;
	[Tooltip("The priority of this chain when mounting. Higher priority chains will be mounted first.")]
	public int MountPriority;

	/// <summary>
	/// The distance between two HingeJoint2D anchors on the same link.
	/// </summary>
	public float LinkAnchorDistance => LinkSize.y - LinkSize.x;
	public float LinkAnchorOffset => LinkAnchorDistance / 2;
	public Rigidbody2D[] Links => m_Links;
	public float Length => m_Links.Length * LinkAnchorDistance;

	private Rigidbody2D[] m_Links;

	private void Start()
	{
		CreateChain(From.position, To.position);
	}

	private void CreateChain(Vector2 fromPoint, Vector2 toPoint)
	{
		var direction = (toPoint - fromPoint).normalized;
		var rotation = Quaternion.FromToRotation(Vector2.up, direction);
		var distance = Mathf.Min(Vector2.Distance(fromPoint, toPoint), MaxLength);
		var links = Mathf.CeilToInt(distance / LinkAnchorDistance);

		m_Links = new Rigidbody2D[links];

		for (var i = 0; i < m_Links.Length; i++)
		{
			var link = CreateLink();

			link.name = $"Link{i}";
			link.transform.parent = transform;
			link.transform.rotation = rotation;
			link.transform.position = fromPoint + direction * i * LinkAnchorDistance;

			m_Links[i] = link;
		}

		for (var i = 1; i < m_Links.Length; i++)
		{
			ConnectLink(m_Links[i], m_Links[i - 1]);
		}

		LinksCreated?.Invoke();
	}

	private Rigidbody2D CreateLink()
	{
		var link = new GameObject();
		link.layer = LayerMask.NameToLayer("Chain");

		var spriteRenderer = link.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = LinkSprite;

		var rigidBody = link.AddComponent<Rigidbody2D>();
		rigidBody.mass = LinkMass;

		var collider = link.AddComponent<CapsuleCollider2D>();
		collider.size = LinkSize;

		return rigidBody;
	}

	private void ConnectLink(Rigidbody2D link, Rigidbody2D previousLink)
	{
		var hingeJoint = link.AddComponent<HingeJoint2D>();
		hingeJoint.connectedBody = previousLink;
		hingeJoint.autoConfigureConnectedAnchor = false;
		hingeJoint.connectedAnchor = new Vector2(0, LinkAnchorOffset);
		hingeJoint.anchor = new Vector2(0, -LinkAnchorOffset);
	}
}
