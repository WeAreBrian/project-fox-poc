using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLevelTransition : MonoBehaviour
{
    [SerializeField]
    private List<float> m_WaterLevels;
    private int m_StageIndex;
    [SerializeField]
    private float m_WaterRisingTime;

    public void RaiseWaterLevel()
    {
        LeanTween.scaleY(gameObject, m_WaterLevels[m_StageIndex], m_WaterRisingTime);
        m_StageIndex++;
    }
}
