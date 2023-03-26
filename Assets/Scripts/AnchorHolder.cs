using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnchorHolder : MonoBehaviour
{
    public bool HoldingAnchor => m_Anchor != null;
    public float GrabRadius = 1;
    public Vector2 HoldPosition = new Vector2(0, 0.5f);

    public Anchor Anchor => m_Anchor;
    public float HoldTime => Time.time - m_HoldStartTime;

    private Anchor m_Anchor;
    private float m_HoldStartTime;

    private void OnAnchorInteract()
    {
        if (!HoldingAnchor)
        {
            GrabAnchor();
        }
    }

    private void GrabAnchor()
    {
        if (HoldingAnchor)
        {
            return;
        }

        var anchorLayerMask = LayerMask.GetMask("Anchor");
        var collider = Physics2D.OverlapCircle(transform.position, GrabRadius, anchorLayerMask);

        Debug.Log(collider);
        if (collider == null)
        {
            return;
        }

        m_Anchor = collider.GetComponent<Anchor>();

        if (m_Anchor == null)
        {
            return;
        }

        var targetJoint = m_Anchor.GetComponent<TargetJoint2D>();

        if (targetJoint == null)
        {
            targetJoint = m_Anchor.AddComponent<TargetJoint2D>();
            targetJoint.autoConfigureTarget = false;
        }

        targetJoint.enabled = true;

        var rigidBody = m_Anchor.GetComponent<Rigidbody2D>();
        rigidBody.gravityScale = 0;

        collider.enabled = false;

        m_Anchor.Unstick();
        
        //Debug.Log("Grabbing Anchor");
        //m_Anchor.PickUp(transform, HoldPosition);

        m_HoldStartTime = Time.time;
    }

    public Anchor DropAnchor()
    {
        if (!HoldingAnchor)
        {
            return null;
        }

        var targetJoint = m_Anchor.GetComponent<TargetJoint2D>();
        targetJoint.enabled = false;

        var rigidBody = m_Anchor.GetComponent<Rigidbody2D>();
        rigidBody.gravityScale = 1;

        var collider = m_Anchor.GetComponent<Collider2D>();
        collider.enabled = true;

        m_Anchor.Drop();

        var anchor = m_Anchor;

        m_Anchor = null;

        return anchor;
    }

    private void FixedUpdate()
    {
        if (m_Anchor != null)
        {
            var targetJoint = m_Anchor.GetComponent<TargetJoint2D>();
            targetJoint.target = (Vector2)transform.position + HoldPosition;
        }
    }
}
