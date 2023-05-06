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

        if (anchor.State == AnchorState.Free)
        {
            if (!grounded.OnGround)
            {
                ApplyForceToFox(0.6f);
                ApplyForceToAnchor(0.4f);

                StartCoroutine(DisableGravity(m_rb, m_NoGravityDuration));
                StartCoroutine(DisableGravity(anchor.Rigidbody, m_NoGravityDuration));
            }
            else
            {
                ApplyForceToAnchor(0.6f);
                StartCoroutine(DisableGravity(anchor.Rigidbody, m_NoGravityDuration));

            }
        }
        else if (anchor.State == AnchorState.Lodged)
        {
            Debug.Log("Lodged Time");
            //anchor.FreeForDuration(0.2f);
            ApplyForceToAnchor(0.6f);
            StartCoroutine(DisableGravity(anchor.Rigidbody, m_NoGravityDuration));


        }
        else
        {
            ApplyForceToFox(1f);
            StartCoroutine(DisableGravity(m_rb, m_NoGravityDuration));

        }

        m_OnCooldown = true;
        StartCoroutine(ResetAbility(m_Cooldown));
        StartCoroutine(AutoAnchorGrab(AutoAttemptAnchorGrabDuration));

    }

    private void ApplyForceToAnchor(float forceCoefficient)
    {
        Debug.Log("pulling anchor");
        var direction = ((Vector3)chain.Tug(anchor.gameObject) - anchor.transform.position).normalized;
        anchor.Rigidbody.velocity = direction * m_TugForce * forceCoefficient;
    }
    private void ApplyForceToFox(float forceCoefficient)
    {
        Debug.Log("pulling fox");
        if (grounded.OnGround) transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);

        var direction = ((Vector3)chain.Tug(gameObject) - transform.position).normalized;
        m_rb.velocity = direction * m_TugForce * forceCoefficient;
    }

    private IEnumerator DisableGravity(Rigidbody2D body, float duration)
    {
        body.gravityScale = 0;
        yield return new WaitForSeconds(duration);
        body.gravityScale = 1;

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
