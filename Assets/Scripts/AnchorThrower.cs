using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorThrower : MonoBehaviour
{
    [SerializeField]
    private AnchorTrajectory m_Trajectory;

    [Tooltip("Min throw velocity and distance")]
    [SerializeField]
    private float m_MinThrowSpeed = 12;

    [Tooltip("Max throw velocity and distance")]
    [SerializeField]
    private float m_MaxThrowSpeed = 12;

    [Tooltip("Time for trajectory to wind up from Min to Max Throw Speed before throwing")]
    [SerializeField]
    [Range(0, 2)]
    private float m_WindUpTime = 0.5f;

    [Tooltip("Rate at which throw speed winds up from Min to Max Throw Speed")]
	[SerializeField]
    private AnimationCurve m_WindUpCurve;

    [Tooltip("Time you can hold while aiming before throw triggers automatically (CURRENTLY BROKEN)")]
	[SerializeField]
    [Range(0, 2)]
    private float m_ThrowHoldTime = 0.2f;

    [Tooltip("After throwing, time in seconds before the Anchor can be thrown again")]
	[SerializeField]
    private float m_ThrowCooldown = 0.2f;

    [Tooltip("Anchor drop velocity after cancelling a hold (click and release). Does not affect velocity of a successful throw")]
	[SerializeField]
    private Vector2 m_DropVelocity = new Vector2(0, 3f);

    [Header("SFX")]
    [SerializeField]
    private AudioClip m_WindUpSound;

    [SerializeField]
    private float m_WindUpSoundInterval;

	public bool WindingUp => m_Trajectory.gameObject.activeSelf;
    public float HoldTime => Time.time - m_WindUpStartTime;
    public float ThrowSpeed => Mathf.Lerp(m_MinThrowSpeed, m_MaxThrowSpeed, m_WindUpCurve.Evaluate(HoldTime / m_WindUpTime));
    public Vector2 ThrowVelocity => m_ThrowDirection * ThrowSpeed;

    private Animator m_animator;
    private AnchorHolder m_Holder;
    private Vector2 m_ThrowDirection;
    private float m_WindUpStartTime;
    private float m_WindUpSoundTimer;
    
    private void Awake()
    {
        m_Trajectory = GetComponentInChildren<AnchorTrajectory>();
        m_Trajectory.gameObject.SetActive(false);

        m_Holder = GetComponent<AnchorHolder>();
        var playerInput = GetComponent<PlayerInput>();
        var anchorInteractAction = playerInput.actions["AnchorInteract"];

        anchorInteractAction.started += OnAnchorInteractStarted;
        anchorInteractAction.canceled += OnAnchorInteractCanceled;

		m_animator = GetComponentInChildren<Animator>();
	}

    private void OnAnchorInteractStarted(InputAction.CallbackContext context)
    {
        if (!m_Holder.HoldingAnchor || m_Holder.HoldTime < m_ThrowCooldown)
        {
            return;
        }

        m_WindUpStartTime = Time.time;
        m_Trajectory.gameObject.SetActive(true);
    }

    private void OnAnchorInteractCanceled(InputAction.CallbackContext context)
    {
        if (!WindingUp)
        {
            return;
        }

        if (HoldTime > m_ThrowHoldTime)
        {
            ThrowAnchor(ThrowVelocity);
        }
        else
        {
            DropAnchor();
        }

        m_Trajectory.gameObject.SetActive(false);

        
    }

    private void OnAim(InputValue value)
    {
        var inputDirection = value.Get<Vector2>();

        if (Mathf.Approximately(inputDirection.sqrMagnitude, 0))
        {
            return;
        }

        m_ThrowDirection = inputDirection;
    }

    private void ThrowAnchor(Vector2 velocity)
    {
        var anchor = m_Holder.DropAnchor();
        anchor.Throw(velocity);
        Debug.Log("Anchor thrown");
        m_animator.SetBool("isThrowing", true);
        AudioController.PlaySound(m_WindUpSound, 1, 1.8f, MixerGroup.SFX);
    }

    private void DropAnchor()
    {
        var anchor = m_Holder.DropAnchor();
        anchor.Throw(m_DropVelocity);
        m_animator.SetBool("isDropping", true);
    }

    private void Update()
    {
        m_Trajectory.Velocity = ThrowVelocity;
    }

    private void FixedUpdate()
    {
        if (WindingUp)
        {
			m_animator.SetBool("isAiming", true);
			OrientAnchor();
            PlayWindupSound();
        }
    }

    private void PlayWindupSound()
    {
        m_WindUpSoundTimer -= Time.fixedDeltaTime;
        if (m_WindUpSoundTimer < 0)
        {
            AudioController.PlaySound(m_WindUpSound, 0.3f, 1.4f, MixerGroup.SFX);
            m_WindUpSoundTimer = m_WindUpSoundInterval;
        }
    }

    private void OrientAnchor(float strength = 4, float damping = 1)
    {
        var anchor = m_Holder.Anchor.Rigidbody;
        var angle = Vector2.SignedAngle(-anchor.transform.up, m_ThrowDirection);

        anchor.AddTorque(-anchor.angularVelocity * damping);
        anchor.AddTorque(angle * strength);
    }

    private void OnDestroy()
    {
        var playerInput = GetComponent<PlayerInput>();
        var anchorInteractAction = playerInput.actions["AnchorInteract"];
        anchorInteractAction.started -= OnAnchorInteractStarted;
        anchorInteractAction.canceled -= OnAnchorInteractCanceled;
    }
}
