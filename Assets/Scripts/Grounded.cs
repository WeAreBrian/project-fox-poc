using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{
    public bool OnGround { get; private set; }

    [SerializeField]
    private LayerMask m_GroundMask;
    private const float k_EdgeOffset = 0.4f;
    private const float k_Height = 0.2f;
    private Collider2D m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        var playerBottom = (Vector2)transform.position - new Vector2(0, m_Collider.bounds.extents.y);
        
        var playerWidth = m_Collider.bounds.extents.x;
        var boxSize = new Vector2(2 * playerWidth - k_EdgeOffset, k_Height);

        OnGround = Physics2D.OverlapBox(playerBottom, boxSize, 0, m_GroundMask);
    }

    private void OnDrawGizmosSelected()
    {
        var collider = GetComponent<Collider2D>();
        var playerBottom = (Vector2)transform.position - new Vector2(0, collider.bounds.extents.y);
        var playerWidth = collider.bounds.extents.x;
        var boxSize = new Vector2(2 * playerWidth - k_EdgeOffset, k_Height);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerBottom, boxSize);
    }
}
