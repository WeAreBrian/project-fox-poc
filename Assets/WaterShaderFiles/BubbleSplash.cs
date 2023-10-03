using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSplash : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystem;
    [SerializeField]
    private float duration = 2.0f; // Duration for which the particle system should play. Set your desired time here.

    private float elapsedTime = 0.0f; // Timer to track the elapsed time since the particle system started
    private bool shouldPlay = false; // Flag to determine if particle system should be playing

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "WaterVolumeForPlayer" || collision.gameObject.name == "WaterVolume")
        {
            particleSystem.Play();
            elapsedTime = 0.0f; // Reset the timer
            shouldPlay = true;  // Set the flag to true
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "WaterVolumeForPlayer" || collision.gameObject.name == "WaterVolume")
        {
            particleSystem.Stop();
            shouldPlay = false; // Set the flag to false
        }
    }

    private void Update()
    {
        if (shouldPlay)
        {
            elapsedTime += Time.deltaTime; // Increment the timer by time elapsed since last frame
            if (elapsedTime >= duration)
            {
                particleSystem.Stop();    // Stop the particle system
                shouldPlay = false;       // Set the flag to false
            }
        }
    }
}