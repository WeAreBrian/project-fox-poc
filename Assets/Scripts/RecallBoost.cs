using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecallBoost : MonoBehaviour
{
    private GameObject m_Anchor;
    private GameObject m_Fox;
    private Rigidbody2D m_AnchorRB;
    private Rigidbody2D m_FoxRB;
    private InputAction m_RecallAction;

    [SerializeField]
    private bool m_OnlyBoostGrapples = false;
    [SerializeField]
    private float m_MultiplyFoxsVelocity = 1.3f;
    [SerializeField]
    private float m_AddGrappledVerticalForceMax = 100f;
    [SerializeField]
    private float m_AddForceToFoxsMoveDirection = 500f;
    [SerializeField]
    private float m_AddForceFromAnchorDirection = 200f;
    
    


    private void Awake()
    {
        m_Anchor = GameObject.FindGameObjectWithTag("Anchor");
        m_Fox = GameObject.FindGameObjectWithTag("Player");
        m_AnchorRB = m_Anchor.GetComponent<Rigidbody2D>();
        m_FoxRB = m_Fox.GetComponent<Rigidbody2D>();

        m_RecallAction = GetComponent<PlayerInput>().actions["Recall"];
        m_RecallAction.performed += ctx => RecallPressed();


    }

    private void RecallPressed()
    {
        if (m_OnlyBoostGrapples)
        {
            if (m_Anchor.GetComponent<Anchor>().State == AnchorState.Lodged)
            {
                BoostVelocity();
            }
        }
        else
        {
            BoostVelocity();
        }
    }

    private void Update()
    {
        Vector2 m_AnchorForceDirection = m_Fox.transform.position - m_Anchor.transform.position;

        //Debug.Log(m_Anchor.GetComponent<Anchor>().State);
        
    }

    private void BoostVelocity()
    {
        Vector2 m_AnchorForceDirection = m_Fox.transform.position - m_Anchor.transform.position;
        Vector2 m_FoxForceDirection = m_FoxRB.velocity.normalized;

        m_FoxRB.velocity *= m_MultiplyFoxsVelocity;


        m_FoxRB.AddForce(m_FoxForceDirection * m_AddForceToFoxsMoveDirection);

        if (m_Anchor.GetComponent<Anchor>().State == AnchorState.Lodged)
        {
            
            m_FoxRB.AddForce(Vector2.up * CalculateVerticalForceAdditionWhenGrappling());
        }

        m_FoxRB.AddForce(m_AnchorForceDirection * m_AddForceFromAnchorDirection);
        
    }

    private float CalculateVerticalForceAdditionWhenGrappling()
    {
        Vector2 m_AnchorForceDirection = m_Fox.transform.position - m_Anchor.transform.position;

        // Calculate the angle in radians using Mathf.Atan2
        float m_AngleRadians = Mathf.Atan2(m_AnchorForceDirection.y, m_AnchorForceDirection.x);

        // Convert the angle from radians to degrees
        float m_AngleDegrees = m_AngleRadians * Mathf.Rad2Deg;

        //Get absolute angle from where down is 0degrees
        float m_Angle = Mathf.Abs(Mathf.Abs(m_AngleDegrees) - 90f);

        Debug.Log(Mathf.Lerp(0f, m_AddGrappledVerticalForceMax, m_Angle / 90f));
        

        // Map the angle value to the range.
        return Mathf.Lerp(0f, m_AddGrappledVerticalForceMax, m_Angle / 90f);
    }
}
