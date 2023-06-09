using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorThrower : MonoBehaviour
{
    public float MinThrowSpeed = 8;
    public float MaxThrowSpeed = 15;
    [Range(0, 2)]
    public float WindUpTime = 0.5f;
    public AnimationCurve WindUpCurve;
    [Range(0, 2)]
    public float ThrowHoldTime = 0.2f;
    public float ThrowCooldown = 0.2f;
    public Vector2 DropVelocity = new Vector2(0, 1.5f);

	public bool WindingUp => m_Trajectory.gameObject.activeSelf;
    public float HoldTime => Time.time - m_WindUpStartTime;
    public float ThrowSpeed => Mathf.Lerp(MinThrowSpeed, MaxThrowSpeed, WindUpCurve.Evaluate(HoldTime / WindUpTime));
    public Vector2 ThrowVelocity => m_ThrowDirection * ThrowSpeed;

    [SerializeField]
    private AnchorTrajectory m_Trajectory;
    private AnchorHolder m_Holder;
    private Vector2 m_ThrowDirection;
    private float m_WindUpStartTime;

    [SerializeField]
    private AudioClip m_WindUpSound;
    [SerializeField]
    private float m_WindUpSoundInterval;
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
	}

    private void OnAnchorInteractStarted(InputAction.CallbackContext context)
    {
        if (!m_Holder.HoldingAnchor || m_Holder.HoldTime < ThrowCooldown)
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

        if (HoldTime > ThrowHoldTime)
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
        AudioController.PlaySound(m_WindUpSound, 1, 1.8f, MixerGroup.SFX);
    }

    private void DropAnchor()
    {
        var anchor = m_Holder.DropAnchor();
        anchor.Throw(DropVelocity);
    }

    private void Update()
    {
        m_Trajectory.Velocity = ThrowVelocity;
    }

    private void FixedUpdate()
    {
        if (WindingUp)
        {
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
