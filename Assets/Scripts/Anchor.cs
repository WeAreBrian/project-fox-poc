 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    public bool Simulated { set => m_Rigidbody.simulated = value; }

    private Rigidbody2D m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Unstick()
    {
        m_Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Throw(Vector2 velocity)
    {
        m_Rigidbody.velocity = velocity;
    }
}
