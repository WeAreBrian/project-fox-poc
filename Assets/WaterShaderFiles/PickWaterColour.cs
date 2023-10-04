using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PickWaterColour : MonoBehaviour
{
    [SerializeField]
    private SpriteShapeRenderer m_Water;

    private void OnValidate()
    {
        if (gameObject.activeInHierarchy)
        {
            GetComponent<ParticleSystem>().startColor = m_Water.color;
        }
    }
}
