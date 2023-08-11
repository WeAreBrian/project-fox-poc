using UnityEngine;
using UnityEngine.InputSystem;

public class VerticalMovement : MonoBehaviour
{

	public delegate void Trigger();
	public static event Trigger jumped;

	public float JumpForce;

	public bool m_FastFall = false; //A public variable for enabling or disabling fast fall. E.g. for springboard.

	[HideInInspector]
	public float JumpCoefficient = 1;

	[SerializeField]
	private float m_coyoteTime;
	[SerializeField]
	private float m_jumpBuffer;

	[SerializeField]
	private bool m_debug;

	private Rigidbody2D m_RigidBody;
	private Grounded m_Grounded;
	private AnchorThrower m_Thrower;
	private HorizontalMovement m_HorizontalMovement;

	private bool m_desiredJump;
	private bool m_isJumping;

	private float m_coyoteTimeCounter;
	private float m_jumpBufferCounter;

	[SerializeField]
	private float m_jumpDownForce;
	private bool m_onJumpRelease;

	[SerializeField]
	private AudioClip m_JumpSound;

    [SerializeField]
    private GameObject m_JumpingDustPoof;
    [SerializeField]
    private float m_JumpingDustPoofPlaybackSpeed = 2f;
	[SerializeField]
    private Vector3 m_JumpingDustPoofPosition = new Vector3(0,0,0);
    [SerializeField]
    private Vector3 m_JumpingDustPoofScale = new Vector3(1, 1, 1);
    private AnimationPrefabSpawner m_AnimationPrefabHolder;


    private int m_GroundedTicks;

	private void Awake()
	{
		m_RigidBody = GetComponent<Rigidbody2D>();
		m_Grounded = GetComponent<Grounded>();
		m_Thrower = GetComponent<AnchorThrower>();
		m_AnimationPrefabHolder = GetComponent<AnimationPrefabSpawner>();
    }

    public void TemporarilyDisableFreeFall()	//This is to be used externally in other scripts e.g. springboard
	{
		m_FastFall = false;
		Debug.Log("Free fall Disabled");
	}


    private void Update()
	{
		// If the player wants to jump, but isn't allowed to jump yet (ie. is mid-air, etc.),
		// we'll be nice and hold onto that request for a little more time to process it later
		if (m_desiredJump)
		{
			m_jumpBufferCounter += Time.deltaTime;

			if (m_jumpBufferCounter > m_jumpBuffer)
			{
				//If time exceeds the limit, we'll drop that jump request
				m_desiredJump = false;
				m_jumpBufferCounter = 0;
			}
		}

		if (!m_isJumping && !m_Grounded.OnGround)
		{
			m_coyoteTimeCounter += Time.deltaTime;
		}
		else
		{
			m_coyoteTimeCounter = 0;
		}
	}

	private void FixedUpdate()
	{
		if (m_desiredJump)
		{
			
			DoJump();
		}
		CheckJumpState();

		// If fox is jumping up and player releases jump key, make the jump shorter
		if(m_isJumping && m_RigidBody.velocity.y > 0f && m_onJumpRelease && m_FastFall)
		{
			m_RigidBody.AddForce(Vector2.down * m_jumpDownForce);
		}

		//if (m_Grounded.OnGround)
		//{
		//	if(m_GroundedTicks < 5)
		//	{
		//		m_GroundedTicks++;
  //          }
		//	else
		//	{
  //              m_FastFall = true;
		//		m_GroundedTicks = 0;
  //          }
  //      }
    }

	private void OnJump(InputValue value)
	{
		// Jump key pressed
		if (value.Get<float>() == 1)
		{
			m_desiredJump = true;
		}
		

		// Jump key released
		if(value.Get<float>() == 0)
		{
			if (m_debug) { Debug.Log("OnJumpDown activated"); }
			m_onJumpRelease = true;
		}
	}

	private void DoJump()
	{
		if (m_debug) { Debug.Log("DoJump activated"); }
		// Fox can't jump when aiming the anchor
		if (m_Thrower.WindingUp)
			return;



		// Fox can only jump when grounded or when there's still coyote time
		if (m_Grounded.OnGround || (m_coyoteTimeCounter > 0.03f && m_coyoteTimeCounter < m_coyoteTime))
		{
			m_desiredJump = false;
			m_isJumping = true;
			m_coyoteTimeCounter = 0;

			m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpForce * JumpCoefficient);
			jumped.Invoke();
			AudioController.PlaySound(m_JumpSound, 1, 1, MixerGroup.SFX);

			//Spawn animation prefab using the script
			m_AnimationPrefabHolder.SpawnAnimationPrefab(m_JumpingDustPoof, m_JumpingDustPoofPlaybackSpeed, m_JumpingDustPoofPosition, m_JumpingDustPoofScale);
		}
	}

	private void CheckJumpState ()
	{
		// If fox is falling and touch the ground, it's no longer jumping
		if (m_RigidBody.velocity.y < -0.01f && m_Grounded.OnGround)
		{
			m_isJumping = false;
			m_onJumpRelease = false;
			
		}
	}
}
