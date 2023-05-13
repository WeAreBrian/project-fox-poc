using Unity.VisualScripting;
using UnityEngine;

public class AnchorHolder : MonoBehaviour
{
    [Tooltip("Unit radius the player is allowed to pick up anchor")]
    [SerializeField]
    private float m_GrabRadius = 1;

    [Tooltip("Anchor hold position relative to Fox's position")]
	[SerializeField]
    private Vector2 m_HoldPosition = new Vector2(0, 0.5f);

    [Tooltip("Jump force when holding the anchor. Multiplied to regular jump force.")]
	[SerializeField]
    private float m_JumpMultiplier;

    [SerializeField]
    private VerticalMovement m_WeightedJump;

    public delegate void Trigger();
    public static event Trigger pickup;

    public bool HoldingAnchor => m_Anchor != null;
    public Anchor Anchor => m_Anchor;
    public float HoldTime => Time.time - m_HoldStartTime;

    private Animator m_animator;
    private Anchor m_Anchor;
    private float m_HoldStartTime;

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
		var collider = Physics2D.OverlapCircle(transform.position, m_GrabRadius, anchorLayerMask);

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
		rigidBody.position = transform.position + (Vector3)m_HoldPosition;

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
			targetJoint.target = (Vector2)transform.position + m_HoldPosition;
		}
	}
}
