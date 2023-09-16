using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleFlower : MonoBehaviour
{
    private Collider2D m_Collider;
    private Animator m_Animator;
    private Anchor m_Anchor;
    [SerializeField]
    private float m_StartTimeOffset;

    private void Start()
    {
        m_Collider = GetComponent<Collider2D>();
        m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat("Offset", m_StartTimeOffset / m_Animator.GetCurrentAnimatorClipInfo(0).Length);
    }
    public void EnableCollider()
    {
        m_Collider.enabled = true;
    }

    public void DisableCollider()
    {
        m_Collider.enabled = false;
        if (m_Anchor != null)
        {
            m_Anchor.FreeForDuration(0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Ignore Chain collisions
        if (collision.gameObject.layer == LayerMask.NameToLayer("Chain")) return;

        m_Anchor = collision.gameObject.GetComponent<Anchor>();
    }
}
