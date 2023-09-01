using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail2D : MonoBehaviour
{
	private GameObject[] m_Children;

	//If something goes wrong in the future, animator should be the fox sprite with object with the animator in the scene. Target joint should be the last joint in the tail.
	[SerializeField] private Animator m_FoxAnimator;
	[SerializeField] private TargetJoint2D m_TargetJoint = null;
	
	private void Awake()
	{
		// Initialize the array with the children
		m_Children = new GameObject[transform.childCount];
		for (int m_Index = 0; m_Index < transform.childCount; m_Index++)
		{
			m_Children[m_Index] = transform.GetChild(m_Index).gameObject;
		}

		UnparentAllChildren();
	}


	//Make the targetjoints target be behind the player facing direction
	private void FixedUpdate()
	{
		m_TargetJoint.target = m_FoxAnimator.transform.position + new Vector3(m_FoxAnimator.GetFloat("Facing") * -1000, m_FoxAnimator.transform.position.y);
    }

	private void UnparentAllChildren()
	{
		// Loop through all children and unparent them
		foreach (GameObject m_Child in m_Children)
		{
			m_Child.transform.SetParent(null);
		}
	}
}
