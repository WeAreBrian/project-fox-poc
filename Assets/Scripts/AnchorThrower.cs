using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class AnchorThrower : MonoBehaviour
{
    public bool ShowTrajectory;

    public UnityEvent WindUp;
    public UnityEvent Throw;

    public float MinThrowSpeed = 8;
    public float MaxThrowSpeed = 15;
    [Range(0, 2)]
    public float WindUpTime = 0.5f;
    public AnimationCurve WindUpCurve;
    [Range(0, 2)]
    public float ThrowHoldTime = 0.2f;
    public float ThrowCooldown = 0.2f;
    public Vector2 DropVelocity = new Vector2(0, 1.5f);
    public float BulletTimeSpeed = 0.5f;

	public bool WindingUp => m_Trajectory.gameObject.activeSelf || m_AimArrow.activeSelf;
    public float HoldTime => Time.time - m_WindUpStartTime;
    public float ThrowSpeed => Mathf.Lerp(MinThrowSpeed, MaxThrowSpeed, WindUpCurve.Evaluate(HoldTime / WindUpTime));
    public Vector2 ThrowVelocity => m_ThrowDirection * ThrowSpeed;

    public float AimDirection => m_ThrowDirection.x;

    [SerializeField]
    private AnchorTrajectory m_Trajectory;
    [SerializeField]
    private GameObject m_AimArrow;
    private AnchorHolder m_Holder;
    private Grounded m_Grounded;
    private Vector2 m_ThrowDirection;
    private float m_WindUpStartTime;
    private GameObject m_Anchor;

    [SerializeField]
    private AudioClip m_WindUpSound;
    [SerializeField]
    private float m_WindUpSoundInterval;
    private float m_WindUpSoundTimer;

    private PlayerInput playerInput;
    private InputAction anchorInteractAction;

    private void Awake()
    {
        m_Trajectory = GetComponentInChildren<AnchorTrajectory>();
        m_Trajectory.gameObject.SetActive(false);
        m_AimArrow.SetActive(false);

        m_Grounded = GetComponent<Grounded>();
        m_Holder = GetComponent<AnchorHolder>();
        playerInput = GetComponent<PlayerInput>();
        anchorInteractAction = playerInput.actions["AnchorInteract"];

        anchorInteractAction.started += OnAnchorInteractStarted;
        anchorInteractAction.canceled += OnAnchorInteractCanceled;

        m_Anchor = GameObject.FindGameObjectWithTag("Anchor");
	}

    private void OnAnchorInteractStarted(InputAction.CallbackContext context)
    {
        if (!m_Holder.HoldingAnchor || m_Holder.HoldTime < ThrowCooldown)
        {
            return;
        }

        m_WindUpStartTime = Time.time;

        if (ShowTrajectory)
        {
            m_Trajectory.gameObject.SetActive(true);
        }
        else
        {
            m_AimArrow.SetActive(true);
        }

        if (!m_Grounded.OnGround)
        {
            Time.timeScale = BulletTimeSpeed;
        }

        WindUp.Invoke();
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

        WindUpStop();
    }

    private void WindUpStop()
    {


        m_Trajectory.gameObject.SetActive(false);
        m_AimArrow.SetActive(false);

        Time.timeScale = 1;
    }

    private void OnAim(InputValue value)
    {
        var inputDirection = value.Get<Vector2>();

        if (Mathf.Approximately(inputDirection.sqrMagnitude, 0))
        {
            return;
        }

        Vector3 dir = (m_AimArrow.transform.position - transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x);
        Quaternion m_Rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg + 90);
        m_AimArrow.transform.SetPositionAndRotation((Vector2)transform.position + new Vector2(0, 0.5f) +(inputDirection*1.5f), m_Rotation);

        m_ThrowDirection = inputDirection;
        //m_Anchor.transform.rotation = m_Rotation;
    }

    private void ThrowAnchor(Vector2 velocity)
    {
        var anchor = m_Holder.DropAnchor();
        anchor.Throw(velocity);
        Throw.Invoke();
        AudioController.PlaySound(m_WindUpSound, 1, 1.8f, MixerGroup.SFX);
    }

    private void DropAnchor()
    {
        var anchor = m_Holder.DropAnchor();
        anchor.Throw(DropVelocity);
    }

    private void Update()
    {
        if (ShowTrajectory)
        {
            m_Trajectory.Velocity = ThrowVelocity;
        }
    }

    private void FixedUpdate()
    {
        if (WindingUp)
        {
            if (!m_Holder.Anchor)
            {
                WindUpStop();
            }
            else
            {
                OrientAnchor();
                PlayWindupSound();
            }
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

    private void OrientAnchor()
    {
        Vector3 m_Direction = (m_AimArrow.transform.position - transform.position);
        float m_Angle = Mathf.Atan2(m_Direction.y, m_Direction.x);
        Quaternion m_Rotation = Quaternion.Euler(0f, 0f, m_Angle * Mathf.Rad2Deg + 90);

        m_Anchor.transform.rotation = m_Rotation;
    }

    private void OnDestroy()
    {
        var playerInput = GetComponent<PlayerInput>();
        var anchorInteractAction = playerInput.actions["AnchorInteract"];
        anchorInteractAction.started -= OnAnchorInteractStarted;
        anchorInteractAction.canceled -= OnAnchorInteractCanceled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, m_ThrowDirection);
    }

    private void OnDisable()
    {
        anchorInteractAction.started -= OnAnchorInteractStarted;
        anchorInteractAction.canceled -= OnAnchorInteractCanceled;
    }
}
