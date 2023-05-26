using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    private Animator m_Animator;
    private Grounded m_Grounded;
    private AnchorHolder m_AnchorHolder;
    private ChainClimber m_ChainClimber;
    [SerializeField] bool m_debugInfo;
    private Rigidbody2D m_RigidBody;
    private GameObject m_model;
    private float k_WalkSpeedScale = 0.3f;
    private float k_ClimbSpeedScale = 0.5f;
    private bool m_Moving;

    private Camera m_cam;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_Grounded = GetComponent<Grounded>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_model = GetComponentInChildren<Animator>().gameObject;
        m_AnchorHolder = GetComponent<AnchorHolder>();
        m_ChainClimber = GetComponent<ChainClimber>();
        m_cam = Camera.main;
    }

    private void Update()
    {


        m_Animator.SetBool("Grounded", m_Grounded.OnGround);
        m_Animator.SetBool("HoldingAnchor", m_AnchorHolder.HoldingAnchor);
        m_Animator.SetBool("Climbing", m_ChainClimber.Mounted);
        if (m_Grounded.OnGround && m_Moving)
        {
            m_Animator.speed = Mathf.Abs(m_RigidBody.velocity.x) * k_WalkSpeedScale;
        }
        else if (m_ChainClimber.Mounted)
        {
            m_Animator.speed = Mathf.Abs(m_RigidBody.velocity.y);
        }
        else
        {
            m_Animator.speed = 1;
        }
    }

    void OnMove(InputValue value)
    {
        // going left, flip fox rotation to face left
        if (value.Get<float>() < 0f)
        {

            m_model.transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        // going right, flip fox rotation to face right
        else if (value.Get<float>() > 0f)
        {

            m_model.transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        else
        {
            m_model.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        m_Moving = value.Get<float>() != 0;

        m_Animator.SetBool("IsMoving", m_Moving);
    }


}
