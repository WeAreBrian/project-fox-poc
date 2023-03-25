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

        if (collider == null)
        {
            return;
        }

        m_Anchor = collider.GetComponent<Anchor>();

        if (m_Anchor == null)
        {
            return;
        }

        m_Anchor.transform.SetParent(transform);
        m_Anchor.transform.localPosition = HoldPosition;
        m_Anchor.Simulated = false;

        m_HoldStartTime = Time.time;
    }

    public Anchor DropAnchor()
    {
        if (!HoldingAnchor)
        {
            return null;
        }

        // Set parent of anchor to world and keep its world position
        m_Anchor.transform.SetParent(null, true);
        m_Anchor.Simulated = true;
        m_Anchor.Unstick();

        var anchor = m_Anchor;

        m_Anchor = null;

        return anchor;
    }
}
