using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToPlayer : MonoBehaviour
{
    private GameObject m_PlayerFox;

    private void Awake()
    {
        m_PlayerFox = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        transform.position = m_PlayerFox.transform.position;
    }
}
