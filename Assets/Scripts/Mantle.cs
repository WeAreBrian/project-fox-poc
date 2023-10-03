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
        if (m_Grounded.OnGround) return;

        Vector2 leftCheckPosition = (Vector2)transform.position + m_LeftOffset - m_BoxCenterOffset / 2;
        Vector2 leftCheckPositionHeightOffset = leftCheckPosition + m_HeightOffset / 2;

        Vector2 rightCheckPosition = (Vector2)transform.position + m_RightOffset - m_BoxCenterOffset / 2;
        Vector2 rightCheckPositionHeightOffset = rightCheckPosition + m_HeightOffset / 2;

        if (Physics2D.OverlapCircle(leftCheckPosition, m_CollisionRadius, m_GroundLayer))
        {
            if (!Physics2D.OverlapCircle(leftCheckPositionHeightOffset, m_CollisionRadius, m_GroundLayer) && !m_Triggered && !IsOverlappingWithSpringboard(leftCheckPosition, m_CollisionRadius))
            {
                Activate();
            }
        }
        else if (Physics2D.OverlapCircle(rightCheckPosition, m_CollisionRadius, m_GroundLayer))
        {
            if (!Physics2D.OverlapCircle(rightCheckPositionHeightOffset, m_CollisionRadius, m_GroundLayer) && !m_Triggered && !IsOverlappingWithSpringboard(rightCheckPosition, m_CollisionRadius))
            {
                Activate();
            }
        }
        else
        {
            m_Triggered = false;
        }
    }

    //This is to check if the springboard is the object overlapping. Because we don't want to mantle on the diagonal springboards, we want to BOUNCE on them!!!
    private bool IsOverlappingWithSpringboard(Vector2 position, float radius)
    {
        Collider2D collider = Physics2D.OverlapCircle(position, radius, m_GroundLayer);
        return collider != null && collider.gameObject.name.Contains("Springboard");
    }


    private void Activate()
    {
        if (m_Grounded.OnGround) return;
        
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
