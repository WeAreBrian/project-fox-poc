using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformColliderAdjustment : MonoBehaviour
{
    private BoxCollider2D m_boxCollider;
    private SpriteRenderer m_spriteRenderer;
    [SerializeField]
    private float m_sizeOffset;

    // Start is called before the first frame update
    void Start()
    {
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_boxCollider.size = new Vector2(m_spriteRenderer.size.x + m_sizeOffset, m_spriteRenderer.size.y + m_sizeOffset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
