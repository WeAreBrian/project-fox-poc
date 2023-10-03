using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSplash : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystem; // Drag your Particle System here in the Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "WaterVolumeForPlayer")
        {
            particleSystem.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "WaterVolumeForPlayer")
        {
            particleSystem.Stop();
        }
    }
}
