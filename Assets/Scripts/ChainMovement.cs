using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChainMovement : MonoBehaviour
{
    public float ClimbSpeed = 4.5f;
    public float MountRadius = 1;
    public float DismountVelocity = 5;
    public float DismountMaxAngle = 60;
    public bool MountFurthestLink = true;
    [HideInInspector]
    public Chain Chain;

    public bool Mounted => m_Attachment != null;

    private float m_ClimbDirection;
    private int m_CurrentLinkIndex;
    private DistanceJoint2D m_Attachment;
    private Rigidbody2D m_Rigidbody;
    private float m_DismountDirection;
    private List<Collider2D> m_MountableLinks = new List<Collider2D>();
    private Grounded m_Grounded;

    public void Mount()
    {
        if (m_Grounded.OnGround)
        {
            return;
        }

        if (Mounted)
        {
            return;
        }

        Debug.Log($"Mounted to chain");

        if (MountFurthestLink)
        {
            m_CurrentLinkIndex = GetFurthestMountableLinkIndex();
        }
        else
        {
            m_CurrentLinkIndex = Chain.Links.Length - 1;
        }

        var link = Chain.Links[m_CurrentLinkIndex];

        m_Attachment = gameObject.AddComponent<DistanceJoint2D>();
        m_Attachment.connectedBody = link;
        m_Attachment.autoConfigureConnectedAnchor = false;
        m_Attachment.connectedAnchor = Vector2.zero;
        m_Attachment.anchor = Vector2.zero;
        m_Attachment.autoConfigureDistance = false;
        m_Attachment.maxDistanceOnly = true;
        m_Attachment.distance = 0.5f;
    }

    public void Dismount()
    {
        if (!Mounted)
        {
            return;
        }

        Debug.Log("Dismounted from chain");

        Destroy(m_Attachment);
        m_Attachment = null;

        if (!m_Grounded.OnGround)
        {
            m_Rigidbody.velocity = Quaternion.Euler(0, 0, DismountMaxAngle * -m_DismountDirection) * Vector2.up * DismountVelocity;
        }
    }

    public void Climb(float direction)
    {
        m_ClimbDirection = direction;
    }

    private void OnMove(InputValue value)
    {
        m_DismountDirection = value.Get<float>();
    }

    private void OnJump()
    {
        Dismount();
    }

    private void OnMount()
    {
        Mount();
    }

    private void OnClimb(InputValue value)
    {
        var direction = value.Get<float>();
        Climb(direction);
    }

    private void FixedUpdate()
    {
        if (!Mounted)
        {
            return;
        }

        if (m_Grounded.OnGround)
        {
            Dismount();
            return;
        }

        if (!Mathf.Approximately(m_ClimbDirection, 0))
        {
            var velocity = -m_Attachment.connectedBody.transform.up * m_ClimbDirection * ClimbSpeed * Time.fixedDeltaTime;
            m_Rigidbody.MovePosition(transform.position + velocity);
        }
        //var force = -m_Attachment.connectedBody.transform.up * m_ClimbDirection * 80;
        //m_Rigidbody.AddForce(force);

        m_Attachment.connectedAnchor -= new Vector2(0, m_ClimbDirection * ClimbSpeed * Time.fixedDeltaTime);

        HandleLinkTransitions();

        var anchorOffset = m_Attachment.connectedAnchor.y;
        anchorOffset = Mathf.Clamp(anchorOffset, -Chain.LinkAnchorOffset, Chain.LinkAnchorOffset);
        m_Attachment.connectedAnchor = new Vector2(0, anchorOffset);
    }

    private void HandleLinkTransitions()
    {
        if (m_Attachment.connectedAnchor.y > Chain.LinkAnchorOffset && m_CurrentLinkIndex < Chain.Links.Length - 1)
        {
            var nextLink = Chain.Links[++m_CurrentLinkIndex];

            m_Attachment.connectedBody = nextLink;
            m_Attachment.connectedAnchor = new Vector2(0, -Chain.LinkAnchorOffset);
        }
        else if (m_Attachment.connectedAnchor.y < -Chain.LinkAnchorOffset && m_CurrentLinkIndex > 1)
        {
            var prevLink = Chain.Links[--m_CurrentLinkIndex];

            m_Attachment.connectedBody = prevLink;
            m_Attachment.connectedAnchor = new Vector2(0, Chain.LinkAnchorOffset);
        }
    } 

    private int GetFurthestMountableLinkIndex()
    {
        var linkFilter = new ContactFilter2D();
        linkFilter.layerMask = LayerMask.GetMask("Chain");

        var linksCount = Physics2D.OverlapCircle(transform.position, MountRadius, linkFilter, m_MountableLinks);

        return m_MountableLinks.Take(linksCount)
            .Select(x => Array.FindIndex(Chain.Links, y => y == x.GetComponent<Rigidbody2D>()))
            .Where(x => x != -1)
            .DefaultIfEmpty(Chain.Links.Length - 1)
            .Min();
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (!Mounted)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, MountRadius);

            var linkIndex = GetFurthestMountableLinkIndex();
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Chain.Links[linkIndex].transform.position, 0.15f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_Attachment.connectedBody.transform.TransformPoint(m_Attachment.connectedAnchor), 0.15f);

            var slideForce = new Ray(transform.position, -m_Attachment.connectedBody.transform.up * m_ClimbDirection);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(slideForce);
        }
    }
}
