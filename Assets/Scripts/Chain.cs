using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public Vector2 LinkSize = new Vector2(0.3f, 0.9f);
    public float LinkMass = 0.1f;
    public Sprite LinkSprite;
    public float MaxDistanceTolerance = 1;
    public Vector2 AnchorAnchorPoint = new Vector2(0, 0.425f);
    public Vector2 PlayerAnchorPoint = Vector2.zero;
    public bool UseMaxLength;
    public float MaxLength = 20;

    /// <summary>
    /// The distance between two HingeJoint2D anchors on the same link.
    /// </summary>
    public float LinkAnchorDistance => LinkSize.y - LinkSize.x;
    public float LinkAnchorOffset => LinkAnchorDistance / 2;
    public Rigidbody2D[] Links => m_Links;

    private Rigidbody2D[] m_Links;

    private void Start()
    {
        var anchor = GameObject.FindGameObjectWithTag("Anchor").GetComponent<Rigidbody2D>();
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        CreateChain(anchor, player);

        var maxDistance = (UseMaxLength ? MaxLength : Vector2.Distance(anchor.transform.position, player.transform.position)) + MaxDistanceTolerance;
        var distanceJoint = player.AddComponent<DistanceJoint2D>();
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.autoConfigureConnectedAnchor = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.connectedBody = anchor;
        distanceJoint.distance = maxDistance;

        // Add chain to ChainMovement
        var chainMovement = player.GetComponent<ChainMovement>();
        chainMovement.Chain = this;
    }

    private void CreateChain(Rigidbody2D anchor, Rigidbody2D player)
    {
        CreateLinks(anchor.transform.position, player.transform.position);
        ConnectPlayerAndAnchor(anchor, player);
    }

    private void ConnectPlayerAndAnchor(Rigidbody2D anchor, Rigidbody2D player)
    {
        var anchorLink = m_Links[0];
        var anchorHingeJoint = anchorLink.AddComponent<HingeJoint2D>();
        anchorHingeJoint.connectedBody = anchor;
        anchorHingeJoint.autoConfigureConnectedAnchor = false;
        anchorHingeJoint.connectedAnchor = AnchorAnchorPoint;
        anchorHingeJoint.anchor = new Vector2(0, -LinkAnchorOffset);

        var playerLink = m_Links[m_Links.Length - 1];
        var playerHingeJoint = playerLink.AddComponent<HingeJoint2D>();
        playerHingeJoint.connectedBody = player;
        playerHingeJoint.autoConfigureConnectedAnchor = false;
        playerHingeJoint.connectedAnchor = PlayerAnchorPoint;
        playerHingeJoint.anchor = new Vector2(0, LinkAnchorOffset);
    }

    private void CreateLinks(Vector2 fromPoint, Vector2 toPoint)
    {
        var direction = (toPoint - fromPoint).normalized;
        var rotation = Quaternion.FromToRotation(Vector2.up, direction);
        var distance = UseMaxLength ? MaxLength : Vector2.Distance(fromPoint, toPoint);
        var links = Mathf.CeilToInt(distance / LinkAnchorDistance);

        m_Links = new Rigidbody2D[links];

        for (var i = 0; i < m_Links.Length; i++)
        {
            var link = CreateLink();

            link.name = $"Link{i}";
            link.transform.parent = transform;
            link.transform.rotation = rotation;
            link.transform.position = fromPoint + direction * i * LinkAnchorDistance;

            m_Links[i] = link;
        }

        for (var i = 1; i < m_Links.Length; i++)
        {
            ConnectLink(m_Links[i], m_Links[i - 1]);
        }
    }

    private Rigidbody2D CreateLink()
    {
        var link = new GameObject();
        link.layer = LayerMask.NameToLayer("Chain");

        var spriteRenderer = link.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = LinkSprite;
        //spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        //spriteRenderer.size = LinkSize;

        var rigidBody = link.AddComponent<Rigidbody2D>();
        rigidBody.mass = LinkMass;

        var collider = link.AddComponent<CapsuleCollider2D>();
        collider.size = LinkSize;

        return rigidBody;
    }

    private void ConnectLink(Rigidbody2D link, Rigidbody2D previousLink)
    {
        var hingeJoint = link.AddComponent<HingeJoint2D>();
        hingeJoint.connectedBody = previousLink;
        hingeJoint.autoConfigureConnectedAnchor = false;
        hingeJoint.connectedAnchor = new Vector2(0, LinkAnchorOffset);
        hingeJoint.anchor = new Vector2(0, -LinkAnchorOffset);
    }
}
