using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Chain))]
public class ChainAttachment : MonoBehaviour
{
	public float MaxDistanceTolerance = 1;
	public Vector2 FromAnchorPoint = new Vector2(0, 0.425f);
	public Vector2 ToAnchorPoint = Vector2.zero;

	private Chain m_Chain;

	private void Awake()
	{
		m_Chain = GetComponent<Chain>();
		m_Chain.LinksCreated += LinkPlayerAndAnchor;
	}

	private void LinkPlayerAndAnchor()
	{
		var fromRigidBody = m_Chain.From.GetComponent<Rigidbody2D>();
		var toRigidBody = m_Chain.To.GetComponent<Rigidbody2D>();

		var firstLink = m_Chain.Links.First();
		var firstHingeJoint = firstLink.gameObject.AddComponent<HingeJoint2D>();
		firstHingeJoint.connectedBody = fromRigidBody;
		firstHingeJoint.autoConfigureConnectedAnchor = false;
		firstHingeJoint.connectedAnchor = FromAnchorPoint;
		firstHingeJoint.anchor = new Vector2(0, -m_Chain.LinkAnchorOffset);

		var lastLink = m_Chain.Links.Last();
		var lastHingeJoint = lastLink.gameObject.AddComponent<HingeJoint2D>();
		lastHingeJoint.connectedBody = toRigidBody;
		lastHingeJoint.autoConfigureConnectedAnchor = false;
		lastHingeJoint.connectedAnchor = ToAnchorPoint;
		lastHingeJoint.anchor = new Vector2(0, m_Chain.LinkAnchorOffset);

		var maxDistance = m_Chain.Length + MaxDistanceTolerance;
		var distanceJoint = m_Chain.To.gameObject.AddComponent<DistanceJoint2D>();
		distanceJoint.autoConfigureDistance = false;
		distanceJoint.autoConfigureConnectedAnchor = false;
		distanceJoint.maxDistanceOnly = true;
		distanceJoint.connectedBody = fromRigidBody;
		distanceJoint.distance = maxDistance;
	}

    private void OnDisable()
    {
		m_Chain.LinksCreated -= LinkPlayerAndAnchor;

    }
}
