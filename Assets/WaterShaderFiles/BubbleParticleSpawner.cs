using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleParticleSpawner : MonoBehaviour
{
    public ParticleSystem m_Bubbles;

    [SerializeField] private float maxVelocityThreshold = 10.0f;  // Max velocity for which the effect would last the longest
    [SerializeField] private float maxDurationMapping = 2.0f;     // Max duration for the bubble effect based on velocity

    private Coroutine stopCoroutine;
    private float baseDelay = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spring"))
        {
            float durationFactor = Mathf.Clamp(collision.relativeVelocity.magnitude / maxVelocityThreshold, 0, 1);
            float duration = baseDelay + (maxDurationMapping - baseDelay) * durationFactor;
            StartBubbles(duration, collision);
        }
    }

    private void StartBubbles(float duration, Collision2D collision)
    {
        // If coroutine is already running, stop it
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
        }

        // Start the bubbles
        if (!m_Bubbles.isPlaying)
        {
            m_Bubbles.Play();
        }

        if (IsComingFromAbove(collision))
        {
            // Start the new coroutine
            stopCoroutine = StartCoroutine(StopBubblesAfterDelay(duration));
        }
    }

    private IEnumerator StopBubblesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Bubbles.Stop();
    }

    private bool IsComingFromAbove(Collision2D collision)
    {
        // Check if the player's Y velocity is negative and greater in magnitude
        return collision.relativeVelocity.y < 0;
    }
}
