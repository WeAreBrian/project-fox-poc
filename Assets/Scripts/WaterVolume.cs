using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVolume : MonoBehaviour
{
	public float m_WaterDrag = 0.7f;
	public float m_BouyancyForce = 14.0f;
	private List<GameObject> m_IgnoredObjects = new List<GameObject>();
	private List<Rigidbody2D> buoyantObjects = new List<Rigidbody2D>();
	private float m_FakeMassMultiplierForPlayerHoldingAnchor = 11f; //11 cos fox is 10mass and anchor is 100 so 10*11 = 10+100

	private void Start()
	{
		// Subscribe to the LinksCreated event in the Chain class
		Chain linksCreator = FindObjectOfType<Chain>();
		linksCreator.LinksCreated += OnLinksCreated;
	}

	private void OnLinksCreated()
	{
		// Get an array of all child components in the parent hierarchy
		Component[] components = GameObject.Find("AnchorChain").GetComponentsInChildren<Component>();
		foreach (Component component in components)
		{
			m_IgnoredObjects.Add(component.gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		//Add all dem bros who enter the water to a list
		Rigidbody2D otherRigidbody = other.GetComponent<Rigidbody2D>();
		if (otherRigidbody != null)
		{
			//Ignore dem bros that you wanna ignore in the water (e.g. The chains cos they defy physics a little because they are actually low density in-game)
			bool ignoreObject = false;
			foreach (GameObject ignoredObject in m_IgnoredObjects)
			{
				Rigidbody2D ignoredRigidbody = ignoredObject.GetComponent<Rigidbody2D>();
				if (ignoredRigidbody != null && ignoredRigidbody == otherRigidbody)
				{
					ignoreObject = true;
					break;
				}
			}
			if (!ignoreObject)
			{
				buoyantObjects.Add(otherRigidbody);
			}
		}
	}

	//Remove the bros who leave the water from the list of bros 
	private void OnTriggerExit2D(Collider2D other)
	{
		Rigidbody2D otherRigidbody = other.GetComponent<Rigidbody2D>();
		if (otherRigidbody != null)
		{
			buoyantObjects.Remove(otherRigidbody);
		}
	}

	//Apply the physics forces of boiancy
	private void FixedUpdate()
	{
		foreach (Rigidbody2D buoyantObject in buoyantObjects)
		{
			//I put a switch here incase we wanna add other exceptions
			switch (buoyantObject.gameObject.name)
			{
				case "PlayerFox":
					//Check if player is holding anchor
					if (buoyantObject.gameObject.GetComponent<AnchorHolder>().HoldingAnchor)
					{
						//Debug.Log("Holding Anchor");
						ApplyLiquidPhysicsToObject(buoyantObject, m_FakeMassMultiplierForPlayerHoldingAnchor);
					}
					else
					{
						//Debug.Log("Not Holding anchor");
						ApplyLiquidPhysicsToObject(buoyantObject);
					}
					break;
				default:
					ApplyLiquidPhysicsToObject(buoyantObject);
					break;
			}
			
		}
	}

	private void ApplyLiquidPhysicsToObject(Rigidbody2D submergedRigidbody, float fakeMassMultiplier = 1f)
	{
		// Calculate the volume (but really Area cos 2d) of the object
		Collider2D collider = submergedRigidbody.GetComponent<Collider2D>();
		float objectWidth = collider.bounds.size.x;
		float objectHeight = collider.bounds.size.y;
		float objectVolume = objectWidth * objectHeight;

		// Calculate the weight of the water displaced by the object
		float displacedWaterMass = m_BouyancyForce * (objectVolume / fakeMassMultiplier);
		float displacedWaterWeight = displacedWaterMass * Mathf.Abs(Physics2D.gravity.y);

		// Apply buoyancy force
		Vector2 buoyantForce = new Vector2(0.0f, displacedWaterWeight);
		submergedRigidbody.AddForce(buoyantForce);

		// Apply drag force (like air resistance, slows things down when move, faster they move, the greater the resistance)
		Vector2 dragForce = -submergedRigidbody.velocity * m_BouyancyForce * m_WaterDrag;
		submergedRigidbody.AddForce(dragForce);
	}
}
