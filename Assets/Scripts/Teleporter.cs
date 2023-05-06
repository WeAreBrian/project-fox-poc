using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Teleporter for quickly hopping between spots in a level for testing.
public class Teleporter : MonoBehaviour
{
	private static Teleporter[] s_Teleporters; // Array of all teleporters in the scene
	private static int s_CurrentIndex = 0; // Index of the currently selected teleporter
	private static bool s_ScriptActivated = false;
	private string m_ObjectName;
	private int m_IndexOfSelf;
	private static GameObject s_PlayerFox;
	private static GameObject s_Anchor;
	private static List<GameObject> s_AnchorChainLinks = new List<GameObject>();
	private static GameObject s_AnchorChain;
	private static bool s_GotChainLinks = false;
    private static bool s_SentDebugMsg = false;

    private void Start()
	{
		m_ObjectName = gameObject.name;
		// Get all instances of the Teleporter script in the scene
		s_Teleporters = FindObjectsOfType<Teleporter>();

		m_IndexOfSelf = GetTeleporterIndex();
		s_PlayerFox = GameObject.FindGameObjectWithTag("Player");
		s_Anchor = GameObject.FindGameObjectWithTag("Anchor");
		s_AnchorChain = GameObject.Find("AnchorChain");


		// Get an array of all child components in the parent hierarchy
		Component[] components = s_AnchorChain.GetComponentsInChildren<Component>();
		// Add all child game objects to the list
		foreach (Component component in components)
		{
			s_AnchorChainLinks.Add(component.gameObject);
		}


		if (!s_SentDebugMsg)
		{
			Debug.Log("Press 1 or 2 to move between teleport spots for testing");
			s_SentDebugMsg = true;
		}
	}

	//This is just so we can see the teleporter in the editor (not ingame)
	private void OnDrawGizmos()
	{
		// Draw a solid red cube Gizmo around this object
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Gizmos.DrawCube(transform.position, transform.localScale);
	}

	private void Update()
	{
		


		if (GetTeleporterIndex() == s_CurrentIndex && !s_ScriptActivated)
		{
			// Check if the number 1 key has been pressed
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				GetChainLinks();
				Teleport();
				// decrement the current index, and loop back if we reach the end
				s_CurrentIndex = (s_CurrentIndex - 1 + s_Teleporters.Length) % s_Teleporters.Length;
				s_ScriptActivated = true;
			}

			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
                GetChainLinks();
                Teleport();
				// Increment the current index, and loop back to 0 if we reach the end
				s_CurrentIndex = (s_CurrentIndex + 1) % s_Teleporters.Length;
				s_ScriptActivated = true;
			}
		}
		
	}

	//allows script to activate again after all teleporter scripts in level have finished.
	private void LateUpdate()
	{
		s_ScriptActivated = false;
	}

	private int GetTeleporterIndex()
	{
		for (int i = 0; i < s_Teleporters.Length; i++)
		{
			if (s_Teleporters[i].name == m_ObjectName)
			{
				return i;
			}
		}
		// Teleporter not found, return -1 or throw an exception
		return -1;
	}

	private void Teleport()
	{
		Vector3 position = transform.position;
		s_PlayerFox.transform.position = position;
		s_Anchor.transform.position = position;

		//Fixes anchor being frozen after teleporting
		s_Anchor.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

		foreach (GameObject link in s_AnchorChainLinks)
		{
			link.transform.position = position;
		}
	}

	private void GetChainLinks()
	{
		//For some reason this isnt working if put in the Start(). Maybe chains are being created after this script runs. Instead we are running this once when the teleport button is pressed.
		if (!s_GotChainLinks)
		{
			// Get an array of all child components in the parent hierarchy
			Component[] components = s_AnchorChain.GetComponentsInChildren<Component>();
			// Add all child game objects to the list
			foreach (Component component in components)
			{
				s_AnchorChainLinks.Add(component.gameObject);
			}
			s_GotChainLinks = true;
		}
	}
}
