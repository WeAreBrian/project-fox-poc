using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionDelta : MonoBehaviour
{
    //returns the change in position since last frame of this object
    public Vector3 Delta;
    private Vector3 m_PositionLastFrame;

    // Update is called once per frame
    void Update()
    {
        Delta = m_PositionLastFrame - transform.position;
        m_PositionLastFrame = transform.position;
    }
}
