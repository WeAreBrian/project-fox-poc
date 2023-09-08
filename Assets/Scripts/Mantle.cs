using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mantle : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_LeftOffset;
    [SerializeField]
    private Vector2 m_RightOffset;
    [SerializeField]
    private Vector2 m_BoxCenterOffset;
    [SerializeField]
    private Vector2 m_HeightOffset;
    [SerializeField]
    private float m_CollisionRadius;
    [SerializeField]
    private LayerMask m_GroundLayer;
    [SerializeField]
    private float m_MantleForce;

    private bool m_Triggered;

    private Rigidbody2D m_Rigidbody;
    private ChainClimber m_ChainClimber;
    private Grounded m_Grounded;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_ChainClimber = GetComponent<ChainClimber>();
        m_Grounded = GetComponent<Grounded>();
    }

    private void Update()
    {
        if (Physics2D.OverlapCircle((Vector2)transform.position + m_LeftOffset - m_BoxCenterOffset / 2, m_CollisionRadius, m_GroundLayer))
        {
            if (!Physics2D.OverlapCircle((Vector2)transform.position + m_LeftOffset - m_BoxCenterOffset + m_HeightOffset / 2, m_CollisionRadius, m_GroundLayer) && !m_Triggered)
            {
                Activate();
            }
        }
        else if (Physics2D.OverlapCircle((Vector2)transform.position + m_RightOffset - m_BoxCenterOffset / 2, m_CollisionRadius, m_GroundLayer))
        {
            if (!Physics2D.OverlapCircle((Vector2)transform.position + m_RightOffset - m_BoxCenterOffset + m_HeightOffset / 2, m_CollisionRadius, m_GroundLayer) && !m_Triggered)
            {
                Activate();
            }
        }
        else
        {
            m_Triggered = false;
        }
    }
    private void Activate()
    {
        if (m_Grounded.OnGround) return;
        
        Debug.Log("Mantling");
        m_ChainClimber.Dismount();
        m_Rigidbody.AddForce(m_MantleForce*Vector2.up, ForceMode2D.Impulse);
        m_Triggered = true;
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere((Vector2)transform.position + m_LeftOffset - m_BoxCenterOffset / 2, m_CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + m_RightOffset - m_BoxCenterOffset / 2, m_CollisionRadius);

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + m_LeftOffset - m_BoxCenterOffset + m_HeightOffset / 2, m_CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + m_RightOffset - m_BoxCenterOffset + m_HeightOffset / 2, m_CollisionRadius);
    }
}
