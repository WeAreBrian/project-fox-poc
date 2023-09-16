using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail2D : MonoBehaviour
{
    [Header("Tail")]
    [SerializeField] private Transform m_TailTargetAnchorPoint;
    [SerializeField] private Rigidbody2D m_TailRigidbody;
    [SerializeField] private TargetJoint2D m_TargetJoint;
    [SerializeField] private Animator m_FoxAnimator;
    [SerializeField] private GameObject m_Tail;
    [SerializeField] private Rigidbody2D m_FoxRigidbody;

    private int m_FaceDirection;

    private bool m_IsMoving;

    private void UpdateTailPose()
    {
        // Calculate the extrapolated target position of the tail anchor. Prevents the tail from lagging behind player.
        Vector2 targetPosition = m_TailTargetAnchorPoint.position;
        targetPosition += m_FoxRigidbody.velocity * Time.fixedDeltaTime;
        m_TailRigidbody.MovePosition(targetPosition);
    }

    private void FixedUpdate()
    {
        UpdateTailPose();
        if(m_FoxRigidbody.velocity.magnitude > 0.1f)
        {
            m_IsMoving = true;
            SetFacingDirection(0.001f);
        }
        else
        {
            m_IsMoving = false;
            SetFacingDirection(1000f);
        }
        
    }

    //This is the make the tale slowly aim towards the back of the fox through the target joint
    private void SetFacingDirection(float strength = 0.01f)
    {
        //player starts off with no direction until they move, so just set it to -1 if its 0
        if (m_FoxAnimator.GetFloat("Facing") == 0)
        {
            m_FaceDirection = -1;
        }
        else
        {
            m_FaceDirection = (int)m_FoxAnimator.GetFloat("Facing");
        }


        //Make the targetjoints target be behind the player facing direction
        m_TargetJoint.target = m_FoxAnimator.transform.position + new Vector3(m_FaceDirection * -1000, m_FoxAnimator.transform.position.y);
        m_TargetJoint.frequency = strength;
    }
}