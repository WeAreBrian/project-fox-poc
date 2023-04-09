using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatorStateMachine : MonoBehaviour
{

    private Animator m_Animator;
    private Grounded m_Grounded;
    [SerializeField] bool m_debugInfo;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Grounded = GetComponent<Grounded>();
		
	}

    // moving left or right, this doesn't change the rotation of the fox
    // if input value is not 0, then initiate running animation, otherwise the fox is still
    void OnMove(InputValue value)
    {
        if (m_debugInfo) { Debug.Log("Move Input: " + value.Get<float>().ToString()); }
        
        // Run animation does not activate during jumping
		if (m_Animator.GetBool("isJumping") == true)
		{
			return;
		}

        // No input from movement, set isRunning to false
		if (value.Get<float>() == 0f)
        {
            m_Animator.SetBool("isRunning", false);
            return;
        }

        // Change animation to running
        m_Animator.SetBool("isRunning", true);
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
