using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedLines : MonoBehaviour
{
    public float minSpeedThreshold = 10.0f; // The speed below which the image will be fully transparent
    public float maxSpeedThreshold = 100.0f; // The speed above which the image will be fully opaque
    private Rigidbody2D rb2d;
    public Image uiImage;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody2D and SpriteRenderer components
        rb2d = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get the magnitude of the velocity
        float speed = rb2d.velocity.magnitude;

        // Calculate the new alpha value based on the speed
        float newAlpha = 0.0f;
        if (speed >= maxSpeedThreshold)
        {
            newAlpha = 1.0f;
        }
        else if (speed > minSpeedThreshold)
        {
            newAlpha = (speed - minSpeedThreshold) / (maxSpeedThreshold - minSpeedThreshold);
        }

        // Set the new alpha value
        Color newColor = uiImage.color;
        newColor.a = newAlpha;
        uiImage.color = newColor;

        Debug.Log(speed);
    }
}
