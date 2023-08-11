using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private CinemachineVirtualCamera m_VirtualCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_VirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void Shake(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin perlin = m_VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = intensity;
        StartCoroutine(StopShake(duration, perlin));
    }

    private IEnumerator StopShake(float duration, CinemachineBasicMultiChannelPerlin perlin)
    {
        yield return new WaitForSeconds(duration);
        perlin.m_AmplitudeGain = 0;
    }
}
