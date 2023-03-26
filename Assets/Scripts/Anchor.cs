 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Anchor : MonoBehaviour
{
    public Rigidbody2D Rigidbody => m_Rigidbody;

    private Rigidbody2D m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            m_Rigidbody.bodyType = m_Rigidbody.bodyType
                == RigidbodyType2D.Static ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //m_Rigidbody.bodyType = RigidbodyType2D.Static;
    }

    public void Unstick()
    {
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void Throw(Vector2 velocity)
    {
        m_Rigidbody.velocity = velocity;
    }
}
