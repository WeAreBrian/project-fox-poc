using UnityEngine;
using UnityEngine.InputSystem;

public class VerticalMovement : MonoBehaviour
{
    [Tooltip("Force of a jump")]
	[SerializeField]
    private float m_JumpForce;

    [Tooltip("Period of delay before dropping to the ground after walking off a ledge")]
	[SerializeField]
    private float m_CoyoteTime;

    [Tooltip("Period that a jump input will register before landing")]
	[SerializeField]
    private float m_JumpBuffer;

    [Tooltip("Period that a jump input will register before landing")]
    [SerializeField]
    private float m_JumpDownForce;

    [Tooltip("Turn on logging for jumping")]
    [SerializeField]
    private bool m_Debug;

    [Header("SFX")]
    [SerializeField]
    private AudioClip m_JumpSound;

    [HideInInspector]
    public float JumpCoefficient = 1;

    private Rigidbody2D m_RigidBody;
    private Grounded m_Grounded;
    private AnchorThrower m_Thrower;

    private bool m_DesiredJump;
    private bool m_IsJumping;
    private bool m_OnJumpRelease;

    private float m_CoyoteTimeCounter;
    private float m_JumpBufferCounter;

    private void Awake()
	{
		m_RigidBody = GetComponent<Rigidbody2D>();
		m_Grounded = GetComponent<Grounded>();
		m_Thrower = GetComponent<AnchorThrower>();
	}

	private void Update()
	{
		// If the player wants to jump, but isn't allowed to jump yet (ie. is mid-air, etc.),
		// we'll be nice and hold onto that request for a little more time to process it later
		if (m_DesiredJump)
		{
			m_JumpBufferCounter += Time.deltaTime;

			if (m_JumpBufferCounter > m_JumpBuffer)
			{
				//If time exceeds the limit, we'll drop that jump request
				m_DesiredJump = false;
				m_JumpBufferCounter = 0;
			}
		}

		if (!m_IsJumping && !m_Grounded.OnGround)
		{
			m_CoyoteTimeCounter += Time.deltaTime;
		}
		else
		{
			m_CoyoteTimeCounter = 0;
		}
	}

	private void FixedUpdate()
	{
		if (m_DesiredJump)
		{
			
			DoJump();
		}
		CheckJumpState();

		// If fox is jumping up and player releases jump key, make the jump shorter
		if(m_IsJumping && m_RigidBody.velocity.y > 0f && m_OnJumpRelease)
		{
			m_RigidBody.AddForce(Vector2.down * m_JumpDownForce);
		}
	}

	private void OnJump(InputValue value)
	{
		// Jump key pressed
		if (value.Get<float>() == 1)
		{
			m_DesiredJump = true;
		}
		

		// Jump key released
		if(value.Get<float>() == 0)
		{
			if (m_Debug) { Debug.Log("OnJumpDown activated"); }
			m_OnJumpRelease = true;
		}
	}

	private void DoJump()
	{
		if (m_Debug) { Debug.Log("DoJump activated"); }
		// Fox can't jump when aiming the anchor
		if (m_Thrower.WindingUp)
			return;



		// Fox can only jump when grounded or when there's still coyote time
		if (m_Grounded.OnGround || (m_CoyoteTimeCounter > 0.03f && m_CoyoteTimeCounter < m_CoyoteTime))
		{
			m_DesiredJump = false;
			m_IsJumping = true;
			m_CoyoteTimeCounter = 0;

			m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, m_JumpForce * JumpCoefficient);

			AudioController.PlaySound(m_JumpSound, 1, 1, MixerGroup.SFX);
		}
	}

	private void CheckJumpState ()
	{
		// If fox is falling and touch the ground, it's no longer jumping
		if (m_RigidBody.velocity.y < -0.01f && m_Grounded.OnGround)
		{
			m_IsJumping = false;
			m_OnJumpRelease = false;
			
		}
	}
}
