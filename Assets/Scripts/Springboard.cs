using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Springboard : MonoBehaviour, IToggle
{
	[SerializeField]
	private float m_SpringForce = 10f;
	[SerializeField]
	private AudioClip m_ActivateSound;

	//IToggle stuff
	[SerializeField]
	private float m_ResetTime = 1f;
	[SerializeField]
	private LayerMask m_IgnoreLayers;
	private bool m_SelfToggle = true;
	private Anchor m_FreeOnToggle;

	//private SpringboardState m_State;
	private Rigidbody2D[] m_ChainRigidBodiesArray;
	private bool m_GottenChainLinks = false;
	private AnchorHolder m_AnchorHolder;
	private VerticalMovement m_VerticalMovement;
	private Grounded m_Grounded;
	private Anchor m_AnchorScript;
	private Animator m_Animator;

	//private Rigidbody2D[] m_RigidbodiesInTriggerZone;
	private List<Rigidbody2D> m_RigidbodiesInTriggerZone = new List<Rigidbody2D>();

	//debug stuff
	private int count;

	private void Start()
	{
		m_AnchorHolder = GameObject.FindGameObjectWithTag("Anchor").GetComponent<AnchorHolder>();
		m_VerticalMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<VerticalMovement>();
		m_Grounded = GameObject.FindGameObjectWithTag("Player").GetComponent<Grounded>();
		m_AnchorScript = GameObject.FindGameObjectWithTag("Anchor").GetComponent<Anchor>();
		m_Animator = GetComponentInChildren<Animator>();
	}

	private void OnTriggerEnter2D(Collider2D m_Collision)
	{
		//Doing this here just once cos otherwise the chain links aren't spawned yet if I do it in Start()
		if (!m_GottenChainLinks)
		{
			GetChainLinks();
		}

		Anchor anchorScript = m_Collision.gameObject.GetComponent<Anchor>();
		if (anchorScript != null)
		{
			m_FreeOnToggle = anchorScript;
		}

		//Thank you Ideka on UnityAnswers
		if (m_IgnoreLayers == (m_IgnoreLayers | (1 << m_Collision.gameObject.layer))) return;

		//Get the rigidbody of the other collider
		Rigidbody2D m_OtherRigidBody = m_Collision.GetComponent<Rigidbody2D>();
		if (m_OtherRigidBody != null && !IsRigidBodyIsAChainRigidBody(m_OtherRigidBody))    //if collider has a rigidbody and also is not a chainlink
		{

			//Debug.Log("Bounce: " + m_Collision + count++);

			//Adds the rigidbody to the list
			m_RigidbodiesInTriggerZone.Add(m_OtherRigidBody);
			Debug.Log(m_RigidbodiesInTriggerZone.Count);

			if (m_OtherRigidBody != null)
			{
                if (!m_SelfToggle) return;
                Toggle();
			}

			
			
		}
	}

	private void OnTriggerExit2D(Collider2D m_Collision)
	{
		//Get the rigidbody of the other collider
		Rigidbody2D m_OtherRigidBody = m_Collision.GetComponent<Rigidbody2D>();
		if (m_OtherRigidBody != null && !IsRigidBodyIsAChainRigidBody(m_OtherRigidBody))    //if collider has a rigidbody and also is not a chainlink
		{
			m_RigidbodiesInTriggerZone.Remove(m_OtherRigidBody);
			Debug.Log(m_RigidbodiesInTriggerZone.Count);
		}
	}

	// Ignores the chain links
	private bool IsRigidBodyIsAChainRigidBody(Rigidbody2D m_RBToCheck){
		foreach (Rigidbody2D chainLink in m_ChainRigidBodiesArray)
		{
			if (chainLink == m_RBToCheck){
				return true;
			}
		}
		return false;
	}

	// Gets the chain links
	private void GetChainLinks()
	{

		m_ChainRigidBodiesArray = GameObject.Find("PhysicsChain").GetComponentsInChildren<Rigidbody2D>();
		m_GottenChainLinks = true;
	}

	public float GetResetTime()
	{
		return m_ResetTime;
	}

	public void Toggle()
	{
		m_FreeOnToggle?.FreeForDuration(1f);
		m_FreeOnToggle = null;

		//m_State = SpringboardState.Triggered;

		AddForceToRigidBodies();

		m_Animator.SetTrigger("Activate");
		AudioController.PlaySound(m_ActivateSound, 1, 1, MixerGroup.SFX);
	}

	public void DisableSelfToggle()
	{
		m_SelfToggle = false;
	}

	private void AddForceToRigidBodies()
	{
		//Okay, Sach explains time. whats going on here is basically to allow springboards to be rotated and bounce in the
		//rotated direction, as well as reseting the "horizontal" velocity (from the POV of the springboard), we are essentially figuring
		//out the "horizontal" (or paralell?) velocity of the object from the pov of the springboard, setting that to "0" (not really but essentially
		//from the pov of the springboard), and then adding the bounce force in the direction of the springboard. This will prevent inconsistence bounce heights.

		foreach(Rigidbody2D rb in m_RigidbodiesInTriggerZone)
		{
            // The freefall in the verticalmovement script messes up the gravity of the player.
            if (rb.gameObject.name == "PlayerFox")
            {
                m_VerticalMovement.TemporarilyDisableFreeFall();
            }

            // Get the springboard's up direction
            Vector2 bounceUp = transform.up;

			// Calculate the perpendicular direction to the bounce pad
			Vector2 bouncePerpendicular = new Vector2(bounceUp.y, -bounceUp.x);

			// Get the other objects's current velocity
			Vector2 objectVelocity = rb.velocity;

			// Project the objectVelocity onto the perpendicular direction to get the perpendicular velocity
			float perpendicularSpeed = Vector2.Dot(objectVelocity, bouncePerpendicular);
			Vector2 perpendicularVel = perpendicularSpeed * bouncePerpendicular;

			// Set the objects's new velocity. This will keep the objects's perpendicular velocity and set the parallel velocity to 0.
			rb.velocity = perpendicularVel;

            //Adds the force of the springboard to the object. The magic number is just a coeffiecent to try get the springheight variable to be as close to real units as possible.
            rb.AddForce(m_SpringForce * transform.up * rb.mass, ForceMode2D.Impulse);
		}
	}
}
