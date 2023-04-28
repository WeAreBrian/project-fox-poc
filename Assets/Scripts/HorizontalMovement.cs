using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalMovement : MonoBehaviour
{
    public float MoveSpeed => m_Grounded.OnGround ? GroundMoveSpeed : AirMoveSpeed;

    [Tooltip("Speed of the player when in the air")]
    public float AirMoveSpeed = 3f;
    public float MaxVelocityInputThreshold;
    public AnimationCurve CoefficientCurve;
    public float GroundMoveSpeed = 5;
    public Vector2 MovableCheckSize = new Vector2(3, 3);
    
    private Rigidbody2D m_Rigidbody;
    [SerializeField]
    private float directionX;
    private Grounded m_Grounded;
    private AnchorThrower m_Thrower;
    private Rigidbody2D m_Grabbed;
    
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
        m_Thrower = GetComponent<AnchorThrower>();

        var playerInput = GetComponent<PlayerInput>();
        var worldInteractAction = playerInput.actions["WorldInteract"];
        worldInteractAction.performed += _ => OnWorldInteractPressed();
        worldInteractAction.canceled += _ => OnWorldInteractReleased();
    }

    private void OnWorldInteractPressed()
    {
		// Check for IMovables
		var colliders = Physics2D.OverlapBoxAll(transform.position, MovableCheckSize, 0);

		m_Grabbed = colliders.Select(x => x.GetComponent<Rigidbody2D>())
			.Where(x => x != null && x.GetComponent<IMovable>() != null)
			.FirstOrDefault();

        if (m_Grabbed != null)
        {
			m_Grabbed.isKinematic = true;
		}
	}

	private void OnWorldInteractReleased()
	{
        if (m_Grabbed != null)
        {
			m_Grabbed.isKinematic = false;

			m_Grabbed = null;
		}
	}

	private void OnMove(InputValue value)
    {
        directionX = value.Get<float>();
    }

    private void FixedUpdate()
    {
        if (m_Thrower.WindingUp)
        {
            m_Rigidbody.velocity = new Vector2(0, m_Rigidbody.velocity.y);
            return;
        }

        var horizontalAxisValue = directionX;

        if (m_Grounded.OnGround)
        {
            m_Rigidbody.velocity = new Vector2(horizontalAxisValue * MoveSpeed, m_Rigidbody.velocity.y);
        }
        else
        {
            m_Rigidbody.AddForce(new Vector2(directionX * AirMoveSpeed * 40 * GetAirCoefficient(), 0));
        }

        if (m_Grabbed != null)
        {
            m_Grabbed.velocity = m_Rigidbody.velocity;
            //m_Grabbed.MovePosition(m_Grabbed.position + m_Rigidbody.velocity);
        }
    }

    private float GetAirCoefficient()
    {
        var coefficient = 0f;
        if ((directionX > 0 && m_Rigidbody.velocity.x > 0) || (directionX < 0 && m_Rigidbody.velocity.x < 0))
        {
            coefficient = (MaxVelocityInputThreshold-Mathf.Clamp(Mathf.Abs(m_Rigidbody.velocity.x), 0, MaxVelocityInputThreshold))/MaxVelocityInputThreshold;
        }
        else
        {
            coefficient = 1;
        }
        return CoefficientCurve.Evaluate(coefficient);
    }
}
