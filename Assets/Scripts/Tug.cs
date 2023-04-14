using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tug : MonoBehaviour
{
    public float m_NoGravityDuration;
    public float m_TugForce;
    public float m_Cooldown;
    private Rigidbody2D m_rb;
    private Anchor anchor;

    private bool m_OnCooldown;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        anchor = FindObjectOfType<Anchor>();
    }

    private void OnTug()
    {
        if (m_OnCooldown) return;

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
        if (GetComponent<Grounded>().OnGround) transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        Vector2 direction = (anchor.transform.position - transform.position).normalized;
        Debug.Log(direction);
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
}
