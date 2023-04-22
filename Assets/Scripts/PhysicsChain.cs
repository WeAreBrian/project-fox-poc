using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsChain : MonoBehaviour
{
	public float Length = 15;
	public Vector2 LinkSize = new Vector2(0.3f, 0.9f);
	public Sprite LinkSprite;
	public float LinkMass = 1;
	public Rigidbody2D Anchor;
	public Rigidbody2D Player;
	public float TargetJointFrequency = 8;
	public float TargetJointDampingRatio = 1;

	public float LinkAnchorDistance => LinkSize.y - LinkSize.x;
	public float LinkAnchorOffset => LinkAnchorDistance / 2;

	private Rigidbody2D[] m_Links;
	private TargetJoint2D m_AnchorTargetJoint;
	private TargetJoint2D m_PlayerTargetJoint;

	private void Start()
	{
		CreateChain();

		m_AnchorTargetJoint = m_Links.First().gameObject.AddComponent<TargetJoint2D>();
		m_AnchorTargetJoint.anchor = Vector2.zero;
		m_AnchorTargetJoint.autoConfigureTarget = false;
		m_AnchorTargetJoint.target = Anchor.position;
		m_AnchorTargetJoint.frequency = TargetJointFrequency;
		m_AnchorTargetJoint.dampingRatio = TargetJointDampingRatio;

		m_PlayerTargetJoint = m_Links.Last().gameObject.AddComponent<TargetJoint2D>();
		m_PlayerTargetJoint.anchor = Vector2.zero;
		m_PlayerTargetJoint.autoConfigureTarget = false;
		m_PlayerTargetJoint.target = Player.position;
		m_PlayerTargetJoint.frequency = TargetJointFrequency;
		m_PlayerTargetJoint.dampingRatio = TargetJointDampingRatio;
	}

	private void FixedUpdate()
	{
		m_AnchorTargetJoint.target = Anchor.position;
		m_PlayerTargetJoint.target = Player.position;
	}

	private void CreateChain()
	{
		var direction = (Player.position - Anchor.position).normalized;
		var rotation = Quaternion.FromToRotation(Vector2.up, direction);
		var links = Mathf.CeilToInt(Length / LinkAnchorDistance);

		m_Links = new Rigidbody2D[links];

		for (var i = 0; i < m_Links.Length; i++)
		{
			var link = CreateLink();

			link.name = $"Link{i}";
			link.transform.parent = transform;
			link.transform.rotation = rotation;
			link.transform.position = Anchor.position + direction * i * LinkAnchorDistance;

			m_Links[i] = link;
		}

		for (var i = 1; i < m_Links.Length; i++)
		{
			ConnectLink(m_Links[i], m_Links[i - 1]);
		}
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
		var hingeJoint = link.gameObject.AddComponent<HingeJoint2D>();
		hingeJoint.connectedBody = previousLink;
		hingeJoint.autoConfigureConnectedAnchor = false;
		hingeJoint.connectedAnchor = new Vector2(0, LinkAnchorOffset);
		hingeJoint.anchor = new Vector2(0, -LinkAnchorOffset);
	}
}
