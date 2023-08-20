using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecallSlingshot : MonoBehaviour
{
    private GameObject m_Anchor;
    private GameObject m_Fox;
    private Rigidbody2D m_FoxRB;

    [Header("All these values can be negative btw. But the default values are okay.")]
    [Header("But the default values are okay.")]
    [Header("Hover over them for tooltip instructions.")]
    [SerializeField]
    [Tooltip("Ticking this makes it only work when coming off a grapple surface")]
    private bool m_OnlyBoostGrapples = true;
    [SerializeField]
    [Tooltip("Multiplies the fox's current velocity")]
    private float m_MultiplyFoxsVelocity = 1.2f;
    [SerializeField]
    [Tooltip("Adds a vertical force when coming off the grapple surface. If directly below the anchor, it's 0. As you approach 90degrees of the anchor it reaches the max force. So swing far up to slingshot higher.")]
    private float m_AddGrappledVerticalForceMax = 1500f;
    [SerializeField]
    [Tooltip("Adds a base force to the direction the fox is moving")]
    private float m_AddForceToFoxsMoveDirection = 100f;
    [SerializeField]
    [Tooltip("Adds a base force from the direction of the anchor")]
    private float m_AddForceFromAnchorDirection = 300f;
    
    


    private void Start()
    {
        m_Anchor = GameObject.FindGameObjectWithTag("Anchor");
        m_Fox = GameObject.FindGameObjectWithTag("Player");
        m_FoxRB = m_Fox.GetComponent<Rigidbody2D>();
    }

    public void TrySlingshot()
    {
        if (m_OnlyBoostGrapples)
        {
            //Check if anchor is lodged into a grapple surface
            if (m_Anchor.GetComponent<Anchor>().State == AnchorState.Lodged)
            {
                Slingshot();
            }
        }
        else
        {
            Slingshot();
        }
    }

    private void Slingshot()
    {
        //calculate direction of anchor to fox, and fox moving direction.
        Vector2 m_AnchorForceDirection = m_Fox.transform.position - m_Anchor.transform.position;
        Vector2 m_FoxForceDirection = m_FoxRB.velocity.normalized;

        //Multiple fox's velocity with multiplier
        m_FoxRB.velocity *= m_MultiplyFoxsVelocity;

        //Add a base force to the fox's direction
        m_FoxRB.AddForce(m_FoxForceDirection * m_AddForceToFoxsMoveDirection);

        //Check if anchor is lodged
        if (m_Anchor.GetComponent<Anchor>().State == AnchorState.Lodged)
        {
            //Add vertical force off the grapple. This makes a slingshot feeling.
            m_FoxRB.AddForce(Vector2.up * CalculateVerticalForceAdditionWhenGrappling());
        }
        //else
        //{
        //    //m_FoxRB.AddForce(Vector2.up * CalculateVerticalForceAdditionWhenGrappling());
        //}

        //Add a force from the anchor to the fox direction. This gives an impact feeling, gives anchor weight.
        m_FoxRB.AddForce(m_AnchorForceDirection * m_AddForceFromAnchorDirection);
        
    }

    private float CalculateVerticalForceAdditionWhenGrappling()
    {
        /*This function works out an angle that the fox is relative to the anchor.
        If the fox is not swinging (directly below the anchor, chain is a vertical line), then force is 0.
        If fox is swinging all the way to be same height as the anchor, it will be max force.
        Inbetween those angles it will return a force between 0 and the max. e.g. 45degrees will be half the max force.
         */

        Vector2 m_AnchorForceDirection = m_Fox.transform.position - m_Anchor.transform.position;

        // Calculate the angle in radians using Mathf.Atan2
        float m_AngleRadians = Mathf.Atan2(m_AnchorForceDirection.y, m_AnchorForceDirection.x);

        // Convert the angle from radians to degrees
        float m_AngleDegrees = m_AngleRadians * Mathf.Rad2Deg;

        //Get absolute angle from where down is 0degrees
        float m_Angle = Mathf.Abs(Mathf.Abs(m_AngleDegrees) - 90f);

        //Debug.Log(Mathf.Lerp(0f, m_AddGrappledVerticalForceMax, m_Angle / 90f));


        // Map the angle value to the range.
        return Mathf.Lerp(0f, m_AddGrappledVerticalForceMax, m_Angle / 90f);

    }
}
