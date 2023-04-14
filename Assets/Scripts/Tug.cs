using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tug : MonoBehaviour
{
    public float m_NoGravityDuration;
    public float m_TugForce;
    public float m_Cooldown;
    private Rigidbody2D m_rb;
    private Anchor anchor;
    private Chain chain;
    private Grounded grounded;
    private AnchorHolder anchorHolder;

    private bool m_OnCooldown;
    private bool m_AutoAttemptAnchorGrab;
    public float AutoAttemptAnchorGrabDuration;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        anchor = FindObjectOfType<Anchor>();
        anchorHolder = GetComponent<AnchorHolder>();
        chain = FindObjectOfType<Chain>();
        grounded = GetComponent<Grounded>();
    }

    private void Update()
    {
        if (m_AutoAttemptAnchorGrab)
        {
            if (anchorHolder.GrabAnchor()) m_AutoAttemptAnchorGrab = false;
        }
    }

    private void OnTug()
    {
        if (m_OnCooldown) return;

        if (anchorHolder.HoldingAnchor) return;

        m_OnCooldown = true;
        anchor.Dislodge();
        StartCoroutine(DisableGravity(m_NoGravityDuration));
        ApplyForce();
        StartCoroutine(ResetAbility(m_Cooldown));
        
    }

    private void ApplyForce()
    {
        //tighten the chain
        //calculate the direction of the next link
        //add force
        if (grounded.OnGround) transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);

        StartCoroutine(AutoAnchorGrab(AutoAttemptAnchorGrabDuration));
        var direction = ((Vector3)chain.Tug() - transform.position).normalized;
        m_rb.velocity = direction*m_TugForce;
    }

    private IEnumerator DisableGravity(float duration)
    {
        m_rb.gravityScale = 0;
        yield return new WaitForSeconds(duration);
        m_rb.gravityScale = 1;

    }

    private IEnumerator ResetAbility(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_OnCooldown = false;
    }

    private IEnumerator AutoAnchorGrab(float duration)
    {
        m_AutoAttemptAnchorGrab = true;
        yield return new WaitForSeconds(duration);
        m_AutoAttemptAnchorGrab = false;
    }
}
