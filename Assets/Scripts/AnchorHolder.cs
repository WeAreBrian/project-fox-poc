using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorHolder : MonoBehaviour
{
	public delegate void Trigger();
	public static event Trigger pickup;

	public bool Surfing;

	public bool HoldingAnchor => m_Anchor != null;
	public float GrabRadius = 1;
	public Vector2 HoldPosition = new Vector2(0, 0.5f);
	public Vector3 HoldRotation = new Vector3(0, 0, 115);

	public Vector2 SurfPosition = new Vector2(0, -0.5f);
	public Vector3 SurfRotation = new Vector3(0, 0, 90);

	public Anchor Anchor => m_Anchor;
	public float HoldTime => Time.time - m_HoldStartTime;

	private Anchor m_Anchor;
	private float m_HoldStartTime;

	[SerializeField]
	private LayerMask m_NotFox;

	[SerializeField]
	private VerticalMovement m_WeightedJump;
	private Grounded m_Grounded;
	public float m_JumpMultiplier;

	private void Awake()
	{
		m_WeightedJump = GetComponent<VerticalMovement>();
		InputAction surf = GetComponent<PlayerInput>().actions["Surf"];
		surf.started += Surf;
		surf.canceled += SurfCancel;

		m_Grounded = GetComponent<Grounded>();

		m_Grounded.Landed.AddListener(StopSurf);
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

		var raycastObject = Physics2D.Raycast(transform.position, (collider.transform.position - transform.position), GrabRadius, m_NotFox);

		if (raycastObject)
		{
			if (raycastObject.collider.gameObject != collider.gameObject)
			{
				return false;
			}
		}

		m_Anchor = collider.GetComponent<Anchor>();

		if (m_Anchor == null)
		{
			return false;
		}


		//if (raycastObject.collider.gameObject != collider.gameObject)
		//{
		//	return false;
		//}
		//Changed to update position to prepare for 3D animation integration 
		//var targetJoint = m_Anchor.GetComponent<TargetJoint2D>();

		//if (targetJoint == null)
		//{
		//	targetJoint = m_Anchor.AddComponent<TargetJoint2D>();
		//	targetJoint.autoConfigureTarget = false;
		//}

		//targetJoint.enabled = true;



		var rigidBody = m_Anchor.GetComponent<Rigidbody2D>();
		rigidBody.gravityScale = 0;
		rigidBody.position = transform.position + (Vector3)HoldPosition;
		m_Anchor.transform.rotation = Quaternion.Euler(HoldRotation);

		collider.enabled = false;

		pickup?.Invoke();
		m_Anchor.PickUp();

		m_WeightedJump.JumpCoefficient = m_JumpMultiplier;
		m_HoldStartTime = Time.time;

		return true;
	}

	private void StopSurf()
	{
		gameObject.layer = LayerMask.NameToLayer("Player");
		Surfing = false;
	}

	private void Surf(InputAction.CallbackContext ctx)
	{
		if (!HoldingAnchor) return;
		if (m_Grounded.OnGround) return;

		gameObject.layer = LayerMask.NameToLayer("Surf");
		Surfing = true;
	}

	private void SurfCancel(InputAction.CallbackContext ctx)
	{
		StopSurf();
	}

	public void ForcePickup()
	{
		m_Anchor = FindObjectOfType<Anchor>();

		var collider = m_Anchor.GetComponent<Collider2D>();
		var rigidBody = m_Anchor.GetComponent<Rigidbody2D>();
		rigidBody.gravityScale = 0;
		rigidBody.position = transform.position + (Vector3)HoldPosition;
		m_Anchor.transform.rotation = Quaternion.Euler(HoldRotation);

		collider.enabled = false;

		pickup?.Invoke();
		m_Anchor.PickUp();

		m_WeightedJump.JumpCoefficient = m_JumpMultiplier;
		m_HoldStartTime = Time.time;
	}

	public Anchor DropAnchor()
	{
		if (!HoldingAnchor) return null;
		m_WeightedJump.JumpCoefficient = 1;

		//var targetJoint = m_Anchor.GetComponent<TargetJoint2D>();
		//targetJoint.enabled = false;

		var rigidBody = m_Anchor.GetComponent<Rigidbody2D>();
		rigidBody.gravityScale = 1;


		m_Anchor.Drop();

		var anchor = m_Anchor;

		m_Anchor = null;

		Surfing = false;

		return anchor;
	}

	private void FixedUpdate()
	{
		if (m_Anchor != null)
		{
			if (Surfing)
            {
				m_Anchor.transform.SetPositionAndRotation((Vector2)transform.position + SurfPosition, Quaternion.Euler(SurfRotation));
            }
            else
            {
				m_Anchor.transform.position = transform.position + (Vector3)HoldPosition;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(transform.position, GameObject.Find("Anchor").transform.position - transform.position);
	}
}
