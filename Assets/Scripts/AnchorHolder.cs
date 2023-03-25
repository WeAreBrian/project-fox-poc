using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorHolder : MonoBehaviour
{
    public bool HoldingAnchor => m_Anchor != null;
    public float GrabRadius = 1;
    public Vector2 HoldPosition = new Vector2(0, 0.5f);

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

        Debug.Log("Grabbing Anchor");
        m_Anchor.PickUp(transform, HoldPosition);

        m_HoldStartTime = Time.time;
    }

    public Anchor DropAnchor()
    {
        if (!HoldingAnchor)
        {
            return null;
        }

        m_Anchor.Drop();

        var anchor = m_Anchor;

        m_Anchor = null;

        return anchor;
    }
}
