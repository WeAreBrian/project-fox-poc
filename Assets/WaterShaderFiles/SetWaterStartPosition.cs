using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWaterStartPosition : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_StartLocation;

    private void OnValidate()
    {
        transform.position = m_StartLocation;
    }
}
