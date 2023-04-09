using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


// Pressure plate is an environmental object that is activated when something heavy is placed on it, like the anchor
public class PressurePlate : MonoBehaviour
{

	private bool m_Active; //whether the pressure plate is active or not
	[SerializeField] private float m_ActivationMass; //the mass needed to activate pressure plate
	[SerializeField] private float m_ActivationCooldown; // a wait time in between activations 
	private bool m_InCooldown;

	[SerializeField] private GameObject[] m_AttachedObjects; //the object(s) it will activate
	private SpriteRenderer m_Sprite;
	private List<Rigidbody2D> m_CollidedObjects;


	// Start is called before the first frame update
	void Start()
	{
		m_Active = false;
		m_Sprite = GetComponent<SpriteRenderer>();
		m_Sprite.color = Color.red;
		m_InCooldown = false;
		m_CollidedObjects = new List<Rigidbody2D>();
		//objects attached to pressure plate will only be activated through pressure plate
		// also attached objects need IToggle interface
		for (int i = 0; i < m_AttachedObjects.Length; i++)
		{
			IToggle toggle = m_AttachedObjects[i].GetComponent<IToggle>();
			toggle.DisableSelfToggle();
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	IEnumerator CooldownCoroutine()
	{

		yield return new WaitForSeconds(m_ActivationCooldown);
		
		m_InCooldown = false;
	}

	// There is a bug when you set the cooldown
	// When it activates and deactivates quickly it may not de/activate correctly
	// An idea to fix is to use fixed update to check for objects at every frame

	// Also when you do not set the cooldown and activate the springboard a lot, it will bounce up to the sky

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (m_InCooldown)
		{
			return;
		}

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
			m_CollidedObjects.Add(collision.GetComponent<Rigidbody2D>());
			Activate();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
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

		if (rb.mass >= m_ActivationMass)
		{
			m_CollidedObjects.Remove(collision.GetComponent<Rigidbody2D>());
		}

		if (m_CollidedObjects.Count == 0)
		{
			Deactivate();
		}
	}

	private void Activate()
	{
		if(m_AttachedObjects.Length > 0)
		{
			for(int i = 0; i < m_AttachedObjects.Length; i++)
			{
				IToggle toggle = m_AttachedObjects[i].GetComponent<IToggle>();
				toggle.Toggle();
			}
		}

		Debug.Log("Pressure plate activated");
		m_Active = true;
		m_Sprite.color = Color.green;
	}

	private void Deactivate()
	{
		Debug.Log("Pressure plate deactivated");
		m_Active = false;
		m_Sprite.color = Color.red;
		m_InCooldown = true;
		StartCoroutine(CooldownCoroutine());
	}
}
