using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail2D : MonoBehaviour
{
    private GameObject[] m_Children;

    [Header("Joint Settings")]
    [SerializeField] private float m_JointAngleLimit = 45f;
    [SerializeField] private float m_JointSpring = 500f;
    [SerializeField] private float m_JointDamper = 10f;

    [Header("Rigidbody Settings")]
    [SerializeField] private float m_RigidbodyMass = 1f;

    [SerializeField] private Animator m_Fox;

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

    private void Update()
    {
        Debug.Log(m_Fox.GetFloat("Facing"));
    }

    private void UnparentAllChildren()
    {
        // Loop through all children and unparent them
        foreach (GameObject m_Child in m_Children)
        {
            // Set the parent to null to unparent
            m_Child.transform.SetParent(null);
        }
    }

    private void ConfigureJointAndRigidbody()
    {
        foreach (GameObject m_Child in m_Children)
        {
            HingeJoint2D m_HingeJoint = m_Child.GetComponent<HingeJoint2D>();
            if (m_HingeJoint != null)
            {
                
            }

            Rigidbody2D m_Rigidbody = m_Child.GetComponent<Rigidbody2D>();
            if (m_Rigidbody != null)
            {
                
            }
        }
    }
}
