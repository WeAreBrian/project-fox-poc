using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalMovement : MonoBehaviour
{
    public float MoveSpeed => m_Grounded.OnGround ? GroundMoveSpeed : AirMoveSpeed;

    [Tooltip("Speed of the player when in the air")]
    public float AirMoveSpeed = 3.5f;
    public float GroundMoveSpeed = 5;
    private Rigidbody2D rb;
    private float directionX;
    private Grounded m_Grounded;
    private AnchorThrower m_Thrower;
    
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
        //rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);

        //if (!Mathf.Approximately(horizontalAxisValue, 0))
        //{
        //    rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);
        //}

        if (m_Grounded.OnGround)
        {
            rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);

            //if (!Mathf.Approximately(horizontalAxisValue, 0))
            //{
            //    rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);
            //}
        }
        else
        {
            rb.AddForce(new Vector2(directionX * 100.0f, 0));
        }
    }
}
