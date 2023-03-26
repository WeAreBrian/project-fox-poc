using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpringboardState
{
    Ready,
    Triggered,
    Resetting
}

public class Springboard : MonoBehaviour, IToggle
{
    private SpringboardState m_State;
    [SerializeField]
    private float m_SpringDistance;
    [SerializeField]
    private float m_SpringResetSpeed;
    [SerializeField]
    private float m_SpringForce;
    [SerializeField]
    private float m_ResetTime;
    private Vector3 m_restingPosition;
    private bool m_SelfToggle = true;
    private Rigidbody2D m_Rb;

    private void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_restingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_State == SpringboardState.Triggered)
        {
            if (transform.localPosition.y >= m_restingPosition.y + m_SpringDistance)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + m_SpringDistance, transform.localPosition.z);
                m_State = SpringboardState.Resetting;
            }
        }
        else if (m_State == SpringboardState.Resetting)
        {
            m_Rb.velocity = Vector2.zero;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - m_SpringResetSpeed, transform.localPosition.z);
            if (transform.localPosition.y <= m_restingPosition.y)
            {
                transform.localPosition = m_restingPosition;
                m_State = SpringboardState.Ready;
            }
        }
    }

    public float GetResetTime()
    {
        return m_ResetTime;
    }

    public void DisableSelfToggle()
    {
        m_SelfToggle = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!m_SelfToggle) return;

        if (m_State == SpringboardState.Resetting) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain")) return;
        
        foreach(ContactPoint2D hitpos in collision.contacts)
        {
            if (hitpos.point.y < GetComponent<Collider2D>().bounds.max.y)
            {
                Debug.Log("not on top");
                return;
            }
        }
        Toggle();
    }

    public void Toggle()
    {
        m_State = SpringboardState.Triggered;
        m_Rb.AddForce(m_SpringForce * Vector2.up, ForceMode2D.Impulse);
    }
}
