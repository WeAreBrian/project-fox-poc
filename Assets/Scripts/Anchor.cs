 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Anchor : MonoBehaviour
{
    public Rigidbody2D Rigidbody => m_Rigidbody;
    
    public AnchorState State => m_State;
    [SerializeField]
    private AnchorState m_State;
    private Rigidbody2D m_Rigidbody;

    private float m_FreeTime;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        m_FreeTime -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Grapplable"))
        {
            UpdateState(AnchorState.Lodged);
        }
        else
        {
            foreach (ContactPoint2D hitpos in collision.contacts)
            {
                var objectBounds = collision.gameObject.GetComponent<Collider2D>().bounds;
                if (hitpos.point.x < objectBounds.min.x+0.1f || hitpos.point.x > objectBounds.max.x-0.1f || hitpos.point.y < objectBounds.min.y+0.1f)
                {
                    Debug.Log("hit a side");
                    UpdateState(AnchorState.Free);
                    return;
                }
            }
            UpdateState(AnchorState.Grounded);
        }
    }

    public void PickUp()
    {
        UpdateState(AnchorState.Held);
    }

    private void UpdateState(AnchorState next)
    {
        Debug.Log("Setting state to " + next);
        if (m_FreeTime > 0)
        {
            Debug.Log("Setting free");
            m_State = AnchorState.Free;
            m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            return;
        }

        //Reset rotation when anchor is picked up
        if (next == AnchorState.Held)
        {
            m_Rigidbody.rotation = 0;
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

    public void Throw(Vector2 velocity)
    {
        m_Rigidbody.velocity = velocity;
    }

    public void FreeForDuration(float seconds)
    {
        m_FreeTime = seconds;
        UpdateState(AnchorState.Free);
    }
}
