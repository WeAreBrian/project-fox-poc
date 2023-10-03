using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToObject : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Target;
    [SerializeField]
    private Vector3 m_Offset;

    private void FixedUpdate()
    {
        transform.position = m_Target.transform.position + m_Offset;
    }
}
