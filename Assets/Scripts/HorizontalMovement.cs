using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalMovement : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;
    private float directionX;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnMove(InputValue value)
    {
        directionX = value.Get<float>();
    }

    private void FixedUpdate()
    {
        float horizontalAxisValue = directionX;
        rb.velocity = new Vector2(horizontalAxisValue * moveSpeed, rb.velocity.y);
    }
}
