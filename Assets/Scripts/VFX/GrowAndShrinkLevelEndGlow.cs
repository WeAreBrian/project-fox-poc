using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowAndShrinkLevelEndGlow : MonoBehaviour
{
    [SerializeField]
    private float frequency = 1.0f;      // Frequency of the sine wave
    [SerializeField]
    private float amplitude = 0.5f;      // Amplitude determines how much it will grow or shrink
    private Vector2 baseScale;            // The default scale of the object in 2D

    private float elapsedTime = 0.0f;   // Time elapsed since the script started

    private void Start()
    {
        baseScale = transform.localScale; // Store the original scale
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        // Calculate the sine value
        float sineValue = Mathf.Sin(elapsedTime * frequency) * amplitude;

        // Apply the sine value to the object's 2D scale
        transform.localScale = new Vector3(baseScale.x + sineValue, baseScale.y + sineValue, 1);
    }
}
