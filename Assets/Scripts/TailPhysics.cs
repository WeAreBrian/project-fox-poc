using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TailTest : MonoBehaviour
{
    //This script should be put on the first bone of the tail object.

    [SerializeField]
    private float m_TailDrag = 0.1f;
    [SerializeField]
    private float m_TailAngularDrag = 0.01f;


    // Start is called before the first frame update
    private void Start()
    {
        //Get an array of the bones
        Transform[] m_Bones = GetComponentsInChildren<Transform>();
        Rigidbody m_RigidBody = gameObject.AddComponent<Rigidbody>();
        m_RigidBody.useGravity = false;
        m_RigidBody.isKinematic = true;

        CharacterJoint m_LastJoint = null;
        //ConfigurableJoint m_LastJoint = null;
        SphereCollider m_LastCollider = null;
        Rigidbody m_LastRigidbody = null;


        //add character joints to each bone
        foreach (Transform m_Bone in m_Bones)
        {
            if (m_Bone.transform != gameObject.transform)
            {
                //ConfigurableJoint m_Joint = m_Bone.transform.gameObject.AddComponent<ConfigurableJoint>();
                CharacterJoint m_Joint = m_Bone.transform.gameObject.AddComponent<CharacterJoint>();
                SphereCollider m_Collider = m_Bone.transform.gameObject.AddComponent<SphereCollider>();
                m_Collider.radius = 0.05f;
                m_Joint.connectedBody = m_Bone.transform.parent.GetComponent<Rigidbody>();
                //m_Joint.enableCollision = true;
                m_Joint.anchor = new Vector3(0, -0.01f, 0); //so the joint anchor isnt in the center of the bone, looks a little nicer like this i think
                Rigidbody m_rb = m_Bone.transform.GetComponent<Rigidbody>();
                m_rb.useGravity = false;
                m_rb.drag = m_TailDrag;
                m_rb.angularDrag = m_TailAngularDrag;

                var m_LimitSprings = m_Joint.twistLimitSpring;
                m_LimitSprings.damper = 5f;
                m_LimitSprings.spring = 30f;
                m_Joint.twistLimitSpring = m_LimitSprings;
                m_Joint.swingLimitSpring = m_LimitSprings;

                var m_LowTwistLimit = m_Joint.lowTwistLimit;
                m_LowTwistLimit.limit = -3f;
                m_LowTwistLimit.contactDistance = 2f;
                m_Joint.lowTwistLimit = m_LowTwistLimit;

                var m_HighTwistLimit = m_Joint.lowTwistLimit;
                m_HighTwistLimit.limit = 3f;
                m_HighTwistLimit.contactDistance = 2f;
                m_Joint.highTwistLimit = m_HighTwistLimit;

                var m_SwingLimit = m_Joint.lowTwistLimit;
                m_SwingLimit.limit = 20f;
                m_SwingLimit.contactDistance = 5f;
                m_Joint.swing1Limit = m_SwingLimit;
                m_Joint.swing2Limit = m_SwingLimit;

                m_Joint.axis = new Vector3(0, -1, 0);
                m_Joint.swingAxis = new Vector3(0, 0, 1);



                m_LastJoint = m_Joint;
                m_LastCollider = m_Collider;
                m_LastRigidbody = m_rb;
            }
            
            
        }

        //Theres a useless bone at the end
        Destroy(m_LastJoint);
        Destroy(m_LastCollider);
        Destroy(m_LastRigidbody);
        Destroy(m_LastJoint.gameObject);

        //Unparent all the bones (when they are parented they snap to follow the fox instead of flow
        foreach (Transform m_Bone in m_Bones)
        {
            if (m_Bone.transform != gameObject.transform)
            {
                m_Bone.transform.parent = null;
            }
        }

        //Parent the end of the tail. I need to do this to minimise jittering and clipping by making the tail go behind the player. The middle bones will still flow
        foreach (Transform m_Bone in m_Bones)
        {
            if (m_Bone.transform != gameObject.transform)
            {
                if(m_Bone == m_Bones[4] || m_Bone == m_Bones[3])
                {
                    m_Bone.transform.parent = transform;
                }
            }
        }

    }
}
