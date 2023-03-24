using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{
    public bool OnGround { get; private set; }

    [SerializeField]
    private LayerMask m_GroundMask;
    private const float k_DistanceOffset = 0.1f;
    private Collider2D m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        var distance = m_Collider.bounds.extents.y + k_DistanceOffset;

        OnGround = Physics2D.Raycast(transform.position, Vector2.down, distance, m_GroundMask);
    }
}
