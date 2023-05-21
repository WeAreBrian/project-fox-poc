using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionDelta : MonoBehaviour
{
    public Vector3 Delta;
    private Vector3 m_PositionLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Delta = m_PositionLastFrame - transform.position;
        m_PositionLastFrame = transform.position;
    }
}
