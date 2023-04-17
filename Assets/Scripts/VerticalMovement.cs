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

    private void OnJump()
    {
        if ((m_Grounded.OnGround || m_coyoteTimeCounter < m_coyoteTime) && !m_Thrower.WindingUp)
        {
            m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpForce*JumpCoefficient);
        }
    }
}
