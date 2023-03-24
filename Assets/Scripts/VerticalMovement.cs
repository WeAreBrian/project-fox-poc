using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    public float JumpForce = 5;

    private Rigidbody2D m_Rigidbody;
    private Grounded m_Grounded;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
    }

    private void OnJump()
    {
        if (m_Grounded.OnGround)
        {
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, JumpForce);
        }
    }
}
