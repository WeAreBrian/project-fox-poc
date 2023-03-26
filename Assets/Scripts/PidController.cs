using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PidController
{
    public float Current;
    public float Target;
    public float Proportional = 2;
    public float Derivative = 0.5f;
    public float Integral = 0.01f;

    public float Error => Target - Current;
    public float Output { get; private set; }

    private float m_ErrorDerivative;
    private float m_ErrorIntegral;
    private float m_PreviousError;

    public float Update(float deltaTime)
    {
        m_ErrorIntegral += Error * deltaTime;
        m_ErrorDerivative = (Error - m_PreviousError) / deltaTime;

        Output = Proportional * Error + Integral * m_ErrorIntegral + Derivative * m_ErrorDerivative;

        m_PreviousError = Error;

        return Output;
    }

    public void Reset()
    {
        m_ErrorDerivative = 0;
        m_ErrorIntegral = 0;
        m_PreviousError = Error;
        Output = 0;
    }
}
