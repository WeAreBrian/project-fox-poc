using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    private Animator m_Animator;
    private Grounded m_Grounded;
    [SerializeField] bool m_debugInfo;
    private Rigidbody2D m_RigidBody;
    private GameObject m_model;


    private Camera m_cam;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_Grounded = GetComponent<Grounded>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_model = m_Animator.gameObject;
        m_cam = Camera.main;
    }

    void OnMove(InputValue value)
    {
        // going left, flip fox rotation to face left
        if (value.Get<float>() == -1f)
        {
            Debug.Log("facing left");
            m_model.transform.rotation = Quaternion.LookRotation(Vector3.left);
        }

        // going right, flip fox rotation to face right
        else if (value.Get<float>() == 1f)
        {
            Debug.Log("facing right");
            m_model.transform.localRotation = Quaternion.LookRotation(Vector3.right);
        }

        else
        {
            m_model.transform.rotation = new Quaternion(0, 180, 0, 0);
        }

        m_Animator.SetBool("IsMoving", value.Get<float>() != 0);
    }
}
