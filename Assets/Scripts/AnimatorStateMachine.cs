using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatorStateMachine : MonoBehaviour
{

    private Animator m_Animator;
    private Grounded m_Grounded;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Grounded = GetComponent<Grounded>();
		m_Animator.SetBool("isIdle", true);
		
	}


    void OnMove(InputValue value)
    {
        m_Animator.SetBool("isRunning", true);
        Debug.Log(value);
    }

    void OnJump(InputValue value)
    {

    }

    void OnClimb(InputValue value)
    {

    }

    void OnMount(InputValue value)
    {

    }

    void OnTug(InputValue value)
    {

    }

    void OnAnchorInteract(InputValue value)
    {

    }

    void OnAim(InputValue value)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        m_Animator.SetBool("isGrounded", m_Grounded.OnGround);
	}
}
