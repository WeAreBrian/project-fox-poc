using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToObject : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Target;

    private void FixedUpdate()
    {
        transform.position = m_Target.transform.position;
    }
}
