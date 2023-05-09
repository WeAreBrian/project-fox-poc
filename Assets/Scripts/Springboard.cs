using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Springboard : MonoBehaviour
{
    [SerializeField]
    private float m_SpringForce = 120f;

    void OnTriggerEnter2D(Collider2D m_Collision)
    {
        Rigidbody2D m_OtherRigidBody = m_Collision.GetComponent<Rigidbody2D>();
        if (m_OtherRigidBody != null)
        {
            m_OtherRigidBody.AddForce(m_SpringForce * Vector2.up, ForceMode2D.Impulse);
        }
    }

}
