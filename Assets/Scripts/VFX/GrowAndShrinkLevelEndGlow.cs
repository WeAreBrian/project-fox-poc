using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowAndShrinkLevelEndGlow : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve growthCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));


    [SerializeField]
    private float m_Frequency = 10f;
    [SerializeField]
    private float m_Amplitude = 0.5f;
    [SerializeField]
    private float m_GrowthAmount = 20f;
    [SerializeField]
    private bool m_TickForGrowOrNotForShrink;
    private Vector2 m_BaseScale;
    private float m_ElapsedTime = 0.0f;

    private bool m_IsShrinking = false;
    private bool m_IsGrowing = false;
    private float m_ProcessDuration;
    private float m_ProcessElapsedTime;
    private Vector2 m_TargetScale;

    private void Start()
    {
        m_BaseScale = transform.localScale;
    }

    private void Update()
    {
        if (!m_IsShrinking && !m_IsGrowing)
        {
            m_ElapsedTime += Time.deltaTime;
            float sineValue = Mathf.Sin(m_ElapsedTime * m_Frequency) * m_Amplitude;
            transform.localScale = new Vector3(m_BaseScale.x + sineValue, m_BaseScale.y + sineValue, 1);
        }
        else
        {
            m_ProcessElapsedTime += Time.unscaledDeltaTime;
            float progress = m_ProcessElapsedTime / m_ProcessDuration;

            if (progress < 1)
            {
                float curveValue = growthCurve.Evaluate(progress);
                Vector2 scaleDifference = m_TargetScale - m_BaseScale;
                transform.localScale = m_BaseScale + scaleDifference * curveValue;
            }
            else
            {
                transform.localScale = m_TargetScale;
                m_BaseScale = transform.localScale;
                m_IsShrinking = m_IsGrowing = false;
            }
        }
    }



    // Trigger growth or shrink process based on the flag m_TickForGrowOrNotForShrink
    public void TriggerGrowOrShrinkEvent(float duration)
    {
        if (m_TickForGrowOrNotForShrink)
        {
            GrowToAmount(duration);
        }
        else
        {
            ShrinkToNothing(duration);
        }
    }

    // Shrink the object to zero scale
    private void ShrinkToNothing(float duration)
    {
        m_IsShrinking = true;
        m_IsGrowing = false;
        m_ProcessDuration = duration;
        m_ProcessElapsedTime = 0.0f;
        m_TargetScale = Vector3.zero;
        m_BaseScale = transform.localScale;
    }

    // Grow the object by m_GrowthAmount
    private void GrowToAmount(float duration)
    {
        m_IsGrowing = true;
        m_IsShrinking = false;
        m_ProcessDuration = duration;
        m_ProcessElapsedTime = 0.0f;
        m_TargetScale = m_BaseScale + new Vector2(m_GrowthAmount, m_GrowthAmount);
        m_BaseScale = transform.localScale;
    }
}
