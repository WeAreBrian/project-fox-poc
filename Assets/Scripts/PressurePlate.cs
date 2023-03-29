using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// Pressure plate is an environmental object that is activated when something heavy is placed on it, like the anchor
public class PressurePlate : MonoBehaviour
{

	private bool m_Active; //whether the pressure plate is active or not
	[SerializeField] private float m_ActivationMass; //the mass needed to activate pressure plate
	[SerializeField] private float m_ActivationCooldown; // a wait time in between activations 
	[SerializeField] private GameObject[] m_AttachedObject; //the object(s) it will activate
	private SpriteRenderer m_Sprite;

	// Start is called before the first frame update
	void Start()
	{
		m_Active = false;
		m_Sprite = GetComponent<SpriteRenderer>();
		m_Sprite.color = Color.green;
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// No foxes can activate this pressure plate

		if (collision.gameObject.CompareTag("Player"))
		{
			return;
		}

		// check if object has a RigidBody
		Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
		if (rb == null)
		{
			return;
		}

		// and if it's mass is equal or higher than activation mass,
		// then activate the pressure plate
		if (rb.mass >= m_ActivationMass)
		{
			Debug.Log("Pressure plate activated");
			m_Active = true;
			m_Sprite.color = Color.green;
		}
	}

	// Same logic as OnCollisionEnter2D

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			return;
		}

		Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
		if (rb == null)
		{
			Debug.Log("no rigid body existing");
			return;
		}

		if (rb.bodyType == RigidbodyType2D.Static)
		{
			Debug.Log("Rb is static");
			return;
		}

		if (rb.mass >= m_ActivationMass)
		{
			Debug.Log("Pressure plate deactivated");
			m_Active = false;
			m_Sprite.color = Color.red;
		}
	}
}
