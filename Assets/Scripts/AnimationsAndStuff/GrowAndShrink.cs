using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowAndShrink : MonoBehaviour
{
    [SerializeField]
    private float m_Amplitude = 1.0f;   // Amplitude of the sine wave
    [SerializeField]
    private float m_Frequency = 1.0f;   // Frequency of the sine wave
    [SerializeField]
    private float m_InitialScaleMultiplier = 1f;

    private Vector3 m_InitialScale;

    private void Start()
    {
        m_InitialScale = transform.localScale;   // Store the initial scale
    }

    private void Update()
    {
        // Calculate the scale change using sine function and time
        float scaleChange = Mathf.Sin(Time.time * m_Frequency) * m_Amplitude;

        // Apply the scale change to the initial scale
        Vector3 newScale = (m_InitialScale * m_InitialScaleMultiplier) + new Vector3(scaleChange, scaleChange, scaleChange);
        transform.localScale = newScale;
    }
}
