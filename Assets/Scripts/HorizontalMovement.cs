using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class HorizontalMovement : MonoBehaviour
{
    [Header("Ground movement")]
    [Tooltip("Fox's speed on the ground")]
    [SerializeField]
    private float m_GroundSpeed = 5f;
    [SerializeField]
    private float m_HoldingAnchorSpeed = 3f;

    [Header("Air movement")]
    [Tooltip("Fox's acceleration in the air (there's a discrepancy because ground movement doesn't have acceleration)")]
    [SerializeField]
    private float m_AirAcceleration = 3f;

    [Tooltip("Fox's max speed to cap off the air acceleration")]
    [SerializeField]
    private float m_MaxAirSpeed;

    private float m_AirSpeedOnLand;
    [Tooltip("How long the player has to jump again after landing to regain airspeed")]
    [SerializeField]
    private float m_BHopWindow;

    private Timer m_BHopTimer;

    [Tooltip("How much ")]
    [SerializeField]
    private AnimationCurve m_AirAccelerationCurve;
    private float MoveSpeed 
    {
        get 
        {
            if (m_Thrower.WindingUp) return 0;
            if (m_Holder.HoldingAnchor) return m_HoldingAnchorSpeed;
            if (!m_Grounded.OnGround) return m_AirAcceleration;
            return m_GroundSpeed;
            
        }
    }

    private Rigidbody2D rb;
    private float directionX;
    private Grounded m_Grounded;
    private AnchorThrower m_Thrower;
    private AnchorHolder m_Holder;
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
        m_Holder = GetComponent<AnchorHolder>();

        m_BHopTimer = new Timer();
        m_BHopTimer.Duration = m_BHopWindow;
        m_BHopTimer.Completed += ResetAirSpeedOnLand;

        m_Grounded.Landed.AddListener(StartBHopTimer);
        VerticalMovement.jumped += AddPreviousAirSpeed;
    }
    
    private void OnMove(InputValue value)
    {
        directionX = value.Get<float>();
    }

    private void Update()
    {
        m_BHopTimer.Tick();
    }

    private void FixedUpdate()
    {
        if (m_Holder.Surfing) return;
 

        var horizontalAxisValue = directionX;
        if (horizontalAxisValue == 0) m_FootstepTimer = m_FootstepInterval;

        if (m_Grounded.OnGround)
        {
            rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);
            PlayFootStepSound();
        }
        else
        {
            rb.AddForce(new Vector2(directionX * MoveSpeed * 40 * GetAirCoefficient() , 0));
        }
    }

    private void StartBHopTimer()
    {
        m_AirSpeedOnLand = rb.velocity.x;
        m_BHopTimer.Start();
    }

    private void ResetAirSpeedOnLand()
    {
        m_AirSpeedOnLand = 0;
        m_BHopTimer.Stop();
    }

    private void AddPreviousAirSpeed()
    {
        if (m_AirSpeedOnLand > 0)
        {
            Debug.Log("BHoppin Baby! Regaining " + m_AirSpeedOnLand);
            StartCoroutine(BHop());
        }
    }

    private IEnumerator BHop()
    {
        yield return new WaitForSeconds(0.03f);

        rb.velocity = new Vector2(Mathf.Max(m_AirSpeedOnLand, rb.velocity.x), rb.velocity.y);
    }

    private float GetAirCoefficient()
    {
        var coefficient = 0f;
        if ((directionX > 0 && rb.velocity.x > 0) || (directionX < 0 && rb.velocity.x < 0))
        {
            coefficient = (m_MaxAirSpeed-Mathf.Clamp(Mathf.Abs(rb.velocity.x), 0, m_MaxAirSpeed))/m_MaxAirSpeed;
        }
        else
        {
            coefficient = 1;
        }
        return m_AirAccelerationCurve.Evaluate(coefficient);
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

    private void OnDisable()
    {
        VerticalMovement.jumped -= AddPreviousAirSpeed;
    }
}
