using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystemDown;
    [SerializeField]
    private ParticleSystem particleSystemRise;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "WaterVolumeForPlayer" || collision.gameObject.name == "WaterVolume")
        {
            particleSystemDown.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "WaterVolumeForPlayer" || collision.gameObject.name == "WaterVolume")
        {
            particleSystemRise.Play();
        }
    }
}