using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Get input (-1 or 1 for A or D)
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        // Set the velocity of the Rigidbody2D based on the input and move speed
        rb.velocity = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
    }
}
