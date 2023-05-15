using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Springboard : MonoBehaviour
{
    [SerializeField]
    private float m_SpringForce = 120f;
    private GameObject m_ChainRigidBodies;

    void Start(){
        //Get the rigidbodys in chain in array
        Rigidbody2D[] m_ChainRigidBodiesArray = GameObject.Find("PhysicsChain").GetComponentsInChildren<Rigidbody2D>();
    }
    void OnTriggerEnter2D(Collider2D m_Collision)
    {
        

        //  AudioController.PlaySound(m_ActivateSound, 1, 1, MixerGroup.SFX);
        Rigidbody2D m_OtherRigidBody = m_Collision.GetComponent<Rigidbody2D>();
        if (m_OtherRigidBody != null && IsRigidBodyIsAChainRigidBody(m_ChainRigidBodies))
        {
            m_OtherRigidBody.AddForce(m_SpringForce * Vector2.up, ForceMode2D.Impulse);
        }
    }

    bool IsRigidBodyIsAChainRigidBody(Rigidbody2D m_RBToCheck){
        foreach (Rigidbody2D rb in m_ChainRigidBodies)
        {
            if (rb == m_RBToCheck){
                return true;
            }
        }
        return false;
    }
}

//I was looking at unity debug log