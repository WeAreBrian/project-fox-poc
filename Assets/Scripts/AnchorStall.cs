using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorStall : MonoBehaviour
{
	private AnchorState m_State;
	private Vector2 m_Velocity;
	private float m_AngularVelocity;
	private Rigidbody2D m_AnchorRigidbody;
	private AnchorHolder m_AnchorHolder;
	private Anchor m_AnchorScript;
	private bool isStalled;

	[SerializeField]
	private float m_StallTime = 1f;
	[SerializeField]
	private bool m_RevertVelocity = true;	//Set this in inspector to false if you want it to fall after ending stall.

	private void Awake()
	{
		//Sets stuff from anchor
		m_AnchorHolder = GetComponent<AnchorHolder>();
		GameObject m_AnchorObject = GameObject.Find("Anchor");
		m_AnchorScript = m_AnchorObject.GetComponent<Anchor>();
		m_AnchorRigidbody = m_AnchorScript.GetComponent<Rigidbody2D>();
	}


	private void OnAnchorInteract()
	{
		//if fox is not holding the anchor and its not already being stalled.
		if (!m_AnchorHolder.HoldingAnchor & !isStalled)
		{
			//Save values
			isStalled = true;
			if (m_RevertVelocity)
			{
				m_Velocity = m_AnchorRigidbody.velocity;
				m_AngularVelocity = m_AnchorRigidbody.angularVelocity;
			}
			
			//Stall
			m_AnchorRigidbody.bodyType = RigidbodyType2D.Static;
			StartCoroutine(WaitCoroutine(m_StallTime));
		}

		//Do this after stall timer
		IEnumerator WaitCoroutine(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			//un-stall after waiting
			m_AnchorRigidbody.bodyType = RigidbodyType2D.Dynamic;
			if (m_RevertVelocity)
			{
				m_AnchorRigidbody.velocity = m_Velocity;
				m_AnchorRigidbody.angularVelocity = m_AngularVelocity;
			}
			isStalled = false;
		}
	}
}