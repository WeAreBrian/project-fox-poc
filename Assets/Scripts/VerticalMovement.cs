using UnityEngine;
using UnityEngine.InputSystem;

public class VerticalMovement : MonoBehaviour
{
    public float JumpForce;
    public float JumpCoefficient = 1;

    [SerializeField]
    private float m_coyoteTime;

    [SerializeField]
    private bool m_debug;

    private Rigidbody2D m_RigidBody;
    private Grounded m_Grounded;
    private float m_coyoteTimeCounter;
    private AnchorThrower m_Thrower;

    [SerializeField]
    private float m_releaseJumpDownForce;
    private bool m_jumpDown;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
        m_Thrower = GetComponent<AnchorThrower>();
    }

    private void Update()
    {
        if (!m_Grounded.OnGround)
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
		if (!m_Grounded.OnGround && m_jumpDown)
		{
            m_RigidBody.AddForce(Vector2.down * m_releaseJumpDownForce);

		}
		else
		{
            m_jumpDown = false;
		}
	}

    private void OnJump(InputValue value)
    {
		
		if (value.Get<float>() == 1 && (m_Grounded.OnGround || m_coyoteTimeCounter < m_coyoteTime) && !m_Thrower.WindingUp)
        {
            if (m_debug) Debug.Log("Jump pressed");
            m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpForce*JumpCoefficient);

			return;
        }

		if (value.Get<float>() == 0 && !m_Grounded.OnGround && m_RigidBody.velocity.y > 0.01f)
        {
			if (m_debug) Debug.Log("Jump down");
            m_jumpDown = true;
		}
    }

}
