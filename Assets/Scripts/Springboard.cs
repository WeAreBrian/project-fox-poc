using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Springboard : MonoBehaviour
{
	[SerializeField]
	private float m_SpringForce = 120f;
	[SerializeField]
	private AudioClip m_ActivateSound;
	private Rigidbody2D[] m_ChainRigidBodiesArray;
	private bool m_GottenChainLinks = false;
	private AnchorHolder m_AnchorHolder;
	private VerticalMovement m_VerticalMovement;
	private Grounded m_Grounded;


	//debug stuff
	private int count;

	private void Start()
	{
		m_AnchorHolder = GameObject.FindGameObjectWithTag("Anchor").GetComponent<AnchorHolder>();
		m_VerticalMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<VerticalMovement>();
		m_Grounded = GameObject.FindGameObjectWithTag("Player").GetComponent<Grounded>();
	}


	void OnTriggerEnter2D(Collider2D m_Collision)
	{
		//Doing this here cos otherwise the chain links aren't spawned yet if I do it in Start()
		if (!m_GottenChainLinks)
		{
			GetChainLinks();
		}
		

		AudioController.PlaySound(m_ActivateSound, 1, 1, MixerGroup.SFX);

		//Get the rigidbody of the other collider
		Rigidbody2D m_OtherRigidBody = m_Collision.GetComponent<Rigidbody2D>();
		if (m_OtherRigidBody != null && !IsRigidBodyIsAChainRigidBody(m_OtherRigidBody))    //if collider has a rigidbody and also is not a chainlink
		{
			Debug.Log("Bounce: " + m_Collision + count++);

			//m_OtherRigidBody.AddForce(m_SpringForce * transform.up, ForceMode2D.Impulse);      //old code

			// Calculate the bounce direction based on the rotation of the bouncepad
			Vector2 bounceDirection = transform.up;

			// Preserve the player's initial velocities except for the bounce direction
			Vector2 preservedVelocity = m_OtherRigidBody.velocity - Vector2.Dot(m_OtherRigidBody.velocity, bounceDirection) * bounceDirection;

			// Apply the preserved velocity and the bounce force
			m_OtherRigidBody.velocity = preservedVelocity + bounceDirection * m_SpringForce;

			if(m_Collision.gameObject.name == "PlayerFox")
			{
				m_VerticalMovement.TemporarilyDisableFreeFall();
			}
		}
	}

	bool IsRigidBodyIsAChainRigidBody(Rigidbody2D m_RBToCheck){
		foreach (Rigidbody2D chainLink in m_ChainRigidBodiesArray)
		{
			if (chainLink == m_RBToCheck){
				return true;
			}
		}
		return false;
	}

	private void GetChainLinks()
	{

		m_ChainRigidBodiesArray = GameObject.Find("PhysicsChain").GetComponentsInChildren<Rigidbody2D>();
		m_GottenChainLinks = true;
	}

	private void Update()
	{
		//Debug.Log(transform.up);
	}
}

//get it to ignore anchor if anchor is held?

//its taking its vertical velocity into account. Maybe just set a velocity in the direction???