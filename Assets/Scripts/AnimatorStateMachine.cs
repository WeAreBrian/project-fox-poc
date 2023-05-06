using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// This isn't a state machine, it uses the controls to set some values in the animator
public class AnimatorStateMachine : MonoBehaviour
{

    private Animator m_Animator;
    private Grounded m_Grounded;
    [SerializeField] bool m_debugInfo;
    private Rigidbody2D m_RigidBody;
    private GameObject m_Sprite;


	private Camera m_cam;

	// Start is called before the first frame update
	void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_Grounded = GetComponent<Grounded>();
		m_RigidBody = GetComponent<Rigidbody2D>();
        m_Sprite = GameObject.Find("Sprite");
        m_cam = Camera.main;
	}

    // moving left or right, this doesn't change the rotation of the fox object
    // if input value is not 0, then initiate running animation, otherwise the fox is still
    void OnMove(InputValue value)
    {
		// going left, flip fox rotation to face left
		if (value.Get<float>() == -1f)
		{
			m_Sprite.transform.localScale = new Vector2(-1, transform.localScale.y);
		}

		// going right, flip fox rotation to face right
		if (value.Get<float>() == 1f)
        {
			m_Sprite.transform.localScale = new Vector2(1, transform.localScale.y);
		}

        if (m_debugInfo) { Debug.Log("Move Input: " + value.Get<float>().ToString()); }
        
        // Run animation does not activate during air time
		//if (m_Animator.GetBool("OnGround"))
        if (m_Grounded.OnGround)
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
        m_Animator.SetBool("isJumping", true);

    }

    private void OnAnchorInteract(InputValue value)
    {
        //m_Animator.SetBool("isPickingUp", true);
    }

	// Update is called once per frame
	void Update()
    {
        
	}

	// in fox aim state, if mouse is towards left of fox, face left, otherwise face right.
	// uses the main camera to determine mouse world position
	private void changeAimDirection()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("FoxAim"))
		{
			if (mouseWorldPos.x < gameObject.transform.position.x)
			{
				m_Sprite.transform.localScale = new Vector2(-1, transform.localScale.y);
			}
			else
			{
				m_Sprite.transform.localScale = new Vector2(1, transform.localScale.y);
			}
		}
	}

    //logic for setting isGrounded
    private void FixedUpdate()
    {
        // from https://stackoverflow.com/questions/51845174/unity-how-do-i-get-my-jump-animation-cycle-to-work																		  
        // get y velocity and round it
        float velY = Mathf.Round((m_RigidBody.velocity.y * 100) / 100);
        
        //update velocityY in Animator
		m_Animator.SetFloat("velocityY", velY);

        //isGrounded is set using the Grounded component
        m_Animator.SetBool("isGrounded", m_Grounded.OnGround);

        // Only set isGrounded to true if y velocity is 0, and fox is touching the ground
        // prevents isGrounded from being pre-emptively set to true after jumping.
        if(m_Animator.GetBool("isGrounded") && velY == 0)
        {
            m_Animator.SetBool("isJumping", false);
        }

        changeAimDirection();
	}

}
