using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HorizontalMovement : MonoBehaviour
{
    [Header("Ground movement")]
    [Tooltip("Movement speed on the ground")]
    [SerializeField]
    private float m_GroundSpeed = 5f;

    [Header("Air movement")]
    [Tooltip("Movement speed in the air")]
    [SerializeField]
    private float m_AirSpeed = 3f;

    [Tooltip("If the Fox is travelling faster than this speed (eg. being shot from a canon), holding down horizontal input won't make him go any faster")]
    [SerializeField]
    private float m_MaxSpeedInputThreshold;

    [Tooltip("How the fox reaches max speed")]
    [SerializeField]
    private AnimationCurve m_AirSpeedCurve;
    private float MoveSpeed => m_Grounded.OnGround ? m_GroundSpeed : m_AirSpeed;

    private Rigidbody2D rb;
    private float directionX;
    private Grounded m_Grounded;
    private AnchorThrower m_Thrower;
    [SerializeField]
    private float m_FootstepInterval;
    private float m_FootstepTimer;
    private bool m_IsLeftFoot;
    [SerializeField]
    private List<AudioClip> m_FootStepSounds;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
        m_Thrower = GetComponent<AnchorThrower>();
    }
    
    private void OnMove(InputValue value)
    {
        directionX = value.Get<float>();
    }

    private void FixedUpdate()
    {
        if (m_Thrower.WindingUp)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        var horizontalAxisValue = directionX;
        if (horizontalAxisValue == 0) m_FootstepTimer = m_FootstepInterval;

        if (m_Grounded.OnGround)
        {
            rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);
            PlayFootStepSound();
        }
        else
        {
            rb.AddForce(new Vector2(directionX * m_AirSpeed * 40 * GetAirCoefficient() , 0));
        }
    }

    private float GetAirCoefficient()
    {
        var coefficient = 0f;
        if ((directionX > 0 && rb.velocity.x > 0) || (directionX < 0 && rb.velocity.x < 0))
        {
            coefficient = (m_MaxSpeedInputThreshold-Mathf.Clamp(Mathf.Abs(rb.velocity.x), 0, m_MaxSpeedInputThreshold))/m_MaxSpeedInputThreshold;
        }
        else
        {
            coefficient = 1;
        }
        return m_AirSpeedCurve.Evaluate(coefficient);
    }

    private void PlayFootStepSound()
    {
        m_FootstepTimer -= Time.deltaTime * Mathf.Abs(directionX);
        if (m_FootstepTimer <= 0)
        {
            var footIndex = Random.Range(0, 2);
            var offset = 0.05f - Random.Range(0, 0.1f);
            AudioController.PlaySound(m_FootStepSounds[footIndex], 0.5f, 1 + offset, MixerGroup.SFX);

            m_FootstepTimer = m_FootstepInterval;
        }
    }
}
