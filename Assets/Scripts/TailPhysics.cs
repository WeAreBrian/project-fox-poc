using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TailTest : MonoBehaviour
{
    //This script should be put on the first bone of the tail object.

    [SerializeField]
    private float m_TailDrag = 0.5f;
    [SerializeField]
    private float m_TailAngularDrag = 0.05f;

    // Start is called before the first frame update
    private void Start()
    {
        //Get an array of the bones
        Transform[] m_Bones = GetComponentsInChildren<Transform>();
        Rigidbody m_RigidBody = gameObject.AddComponent<Rigidbody>();
        m_RigidBody.useGravity = false;
        m_RigidBody.isKinematic = true;

        //add character joints to each bone
        foreach(Transform m_Bone in m_Bones)
        {
            if (m_Bone.transform != gameObject.transform)
            {
                CharacterJoint m_Joint = m_Bone.transform.gameObject.AddComponent<CharacterJoint>();
                m_Joint.connectedBody = m_Bone.transform.parent.GetComponent<Rigidbody>();
                m_Joint.anchor = new Vector3(0, -0.001f, 0); //so the joint anchor isnt in the center of the bone, looks a little nicer like this.

                m_Bone.transform.GetComponent<Rigidbody>().useGravity = false;
                m_Bone.transform.GetComponent<Rigidbody>().drag = m_TailDrag;
                m_Bone.transform.GetComponent<Rigidbody>().angularDrag = m_TailAngularDrag;
            }

        }
        foreach (Transform m_Bone in m_Bones)
        {
            if (m_Bone.transform != gameObject.transform)
            {
                m_Bone.transform.parent = null;
            }
        }

    }
}
