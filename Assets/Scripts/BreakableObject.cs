using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] 
    private GameObject m_brokenWallPieces; 

    [SerializeField] 
    private GameObject m_smokeParticles;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Anchor"))
    	{
            Instantiate(m_brokenWallPieces, transform.position, transform.rotation);
            // set the y position to be whatever direction you're throwing the anchor in
            Instantiate(m_smokeParticles, transform.position, transform.rotation);
            Destroy(gameObject); 
        }
    }
}
