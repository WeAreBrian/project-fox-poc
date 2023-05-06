using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnchorHolder : MonoBehaviour
{
	public delegate void Trigger();
	public static event Trigger pickup;

	public bool HoldingAnchor => m_Anchor != null;
	public float GrabRadius = 1;
	public Vector2 HoldPosition = new Vector2(0, 0.5f);

	public Anchor Anchor => m_Anchor;
	public float HoldTime => Time.time - m_HoldStartTime;

	private Anchor m_Anchor;
	private float m_HoldStartTime;

	[SerializeField]
	private VerticalMovement m_WeightedJump;
	public float m_JumpMultiplier;

	private Animator m_animator;

	private void Awake()
	{
		m_WeightedJump = GetComponent<VerticalMovement>();
		m_animator = GetComponentInChildren<Animator>();
	}

	private void OnAnchorInteract()
	{
		if (!HoldingAnchor)
		{
			GrabAnchor();
		}
	}

	public bool GrabAnchor()
	{
		if (HoldingAnchor) return false;

		var anchorLayerMask = LayerMask.GetMask("Anchor");
		var collider = Physics2D.OverlapCircle(transform.position, GrabRadius, anchorLayerMask);

		if (collider == null)
		{
			return false;
		}

		m_Anchor = collider.GetComponent<Anchor>();

		if (m_Anchor == null)
		{
			return false;
		}

		var targetJoint = m_Anchor.GetComponent<TargetJoint2D>();

		if (targetJoint == null)
		{
			targetJoint = m_Anchor.AddComponent<TargetJoint2D>();
			targetJoint.autoConfigureTarget = false;
		}

		targetJoint.enabled = true;

		var rigidBody = m_Anchor.GetComponent<Rigidbody2D>();
		rigidBody.gravityScale = 0;
		rigidBody.position = transform.position + (Vector3)HoldPosition;

		collider.enabled = false;

		pickup?.Invoke();
		m_Anchor.PickUp();
		m_WeightedJump.JumpCoefficient = m_JumpMultiplier;
		m_HoldStartTime = Time.time;

		
		m_animator.SetBool("isPickingUp", true);
		return true;
	}

	public Anchor DropAnchor()
	{
		if (!HoldingAnchor) return null;
		m_WeightedJump.JumpCoefficient = 1;

		var targetJoint = m_Anchor.GetComponent<TargetJoint2D>();
		targetJoint.enabled = false;

		var rigidBody = m_Anchor.GetComponent<Rigidbody2D>();
		rigidBody.gravityScale = 1;

		var collider = m_Anchor.GetComponent<Collider2D>();
		collider.enabled = true;

		m_Anchor.Drop();

		var anchor = m_Anchor;

		m_Anchor = null;

		return anchor;
	}

	private void FixedUpdate()
	{
		if (m_Anchor != null)
		{
			var targetJoint = m_Anchor.GetComponent<TargetJoint2D>();
			targetJoint.target = (Vector2)transform.position + HoldPosition;
		}
	}
}
