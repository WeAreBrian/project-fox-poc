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
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_Grounded = GetComponent<Grounded>();
    }
    
    private void OnMove(InputValue value)
    {
        directionX = value.Get<float>();
    }

    private void FixedUpdate()
    {
        float horizontalAxisValue = directionX;
        rb.velocity = new Vector2(horizontalAxisValue * MoveSpeed, rb.velocity.y);
    }
}
