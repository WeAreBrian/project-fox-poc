using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    public float JumpForce;

    private Rigidbody2D m_RigidBody;
    private Grounded m_Grounded;
    private AnchorThrower m_Thrower;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
        m_Thrower = GetComponent<AnchorThrower>();
    }

    private void OnJump()
    {
        if (m_Grounded.OnGround && !m_Thrower.WindingUp)
        {
            m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpForce);
        }
    }
}
