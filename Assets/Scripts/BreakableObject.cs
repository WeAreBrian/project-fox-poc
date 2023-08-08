using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject m_brokenWallPieces;

    [SerializeField]
    private GameObject m_smokeParticles;

    [SerializeField]
    private bool m_isVertical;

    Quaternion GetParticleSprayDirection(Collider2D collision)
    {
        Vector2 m_localPos = gameObject.transform.position - collision.gameObject.transform.position;
        if (m_localPos.x > 0 && !m_isVertical) {
            return Quaternion.Euler(-90, 0, 0);
        }

        else if (m_localPos.y > 0 && m_isVertical) {
            return Quaternion.Euler(0, -90, 0);
        }
        else {
            return Quaternion.identity;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Anchor"))
        {
            Instantiate(m_brokenWallPieces, transform.position, GetParticleSprayDirection(collision));
            Instantiate(m_smokeParticles, transform.position, GetParticleSprayDirection(collision));
            Destroy(gameObject);
        }
    }
}
