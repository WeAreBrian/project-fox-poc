using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    public float JumpForce;
    public float JumpCoefficient = 1;

    [SerializeField]
    private float m_coyoteTime;

    private Rigidbody2D m_RigidBody;
    private Grounded m_Grounded;
    private float m_coyoteTimeCounter;
    private AnchorThrower m_Thrower;
    private bool m_desiredJump;
    private bool m_isJumping;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
        m_Thrower = GetComponent<AnchorThrower>();
    }

    private void Update()
    {
        if (!m_isJumping && !m_Grounded.OnGround)
        {
            m_coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            m_coyoteTimeCounter = 0;
        }
    }

    private void FixedUpdate()
    {
        if (m_desiredJump)
        {
            DoJump();
        }
        CheckJumpState();
    }

    private void OnJump()
    {
        m_desiredJump = true;
        DoJump();
    }

    private void DoJump()
    {
        // Fox can't jump when aiming the anchor
        if (m_Thrower.WindingUp)
            return;

        // Fox can only jump when grounded or when there's still coyote time
        if (m_Grounded.OnGround || (m_coyoteTimeCounter > 0.03f && m_coyoteTimeCounter < m_coyoteTime))
        {
            m_desiredJump = false;
            m_isJumping = true;
            m_coyoteTimeCounter = 0;

            m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpForce * JumpCoefficient);
        }
    }

    private void CheckJumpState ()
    {
        // If fox is falling and touch the ground, it's no longer jumping
        if (m_RigidBody.velocity.y < -0.01f && m_Grounded.OnGround)
        {
            m_isJumping = false;
        }
    }
}
