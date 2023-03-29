 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Anchor : MonoBehaviour
{
    public Rigidbody2D Rigidbody => m_Rigidbody;

    private AnchorState m_State;
    private GameObject m_ObjectLastLodgedIn;
    private Rigidbody2D m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    //private void Update()
    //{
    //    if (Keyboard.current.fKey.wasPressedThisFrame)
    //    {
    //        m_Rigidbody.bodyType = m_Rigidbody.bodyType
    //            == RigidbodyType2D.Static ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grapplable") && collision.gameObject != m_ObjectLastLodgedIn)
        {
            UpdateState(AnchorState.Lodged);
            m_ObjectLastLodgedIn = collision.gameObject;
        }
        else
        {
            UpdateState(AnchorState.Grounded);
        }
    }

    public void PickUp()
    {
        UpdateState(AnchorState.Held);
    }

    private void UpdateState(AnchorState next)
    {
        // Reset lodge object so that it can be lodged in again
        if (next == AnchorState.Grounded || next == AnchorState.Held)
        {
            m_ObjectLastLodgedIn = null;
        }

        //set contraints based on state
        if (next == AnchorState.Lodged)
        {
            m_Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            m_Rigidbody.constraints = RigidbodyConstraints2D.None;
        }

        //Reset rotation when anchor is picked up
        if (next == AnchorState.Held)
        {
            Simulated = false;
            m_Rigidbody.rotation = 0;
        }
        else
        {
            Simulated = true;
        }
        // Set body type based on state
        if (next == AnchorState.Free || next == AnchorState.Held)
        {
            m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            m_Rigidbody.bodyType = RigidbodyType2D.Static;
        }

        m_State = next;
    }

    public void Drop()
    {
        UpdateState(AnchorState.Free);
    }

    public void Dislodge()
    {
        UpdateState(AnchorState.Free);
    }

    public void Throw(Vector2 velocity)
    {
        m_Rigidbody.velocity = velocity;
    }
}
