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
        // get local position of colliding obj
        Vector2 m_localPos = gameObject.transform.position - collision.gameObject.transform.position;

        if (m_isVertical)
        {
            if (m_localPos.x > 0)
            {
                return Quaternion.Euler(0, 90, 0);
            }
           return Quaternion.Euler(0,-90, 0);
        }

        //if horizontal wall

		if (m_localPos.y > 0) {
            return Quaternion.Euler(-90, 0, 0);
        }
        
        return Quaternion.Euler(90,0,0);

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
