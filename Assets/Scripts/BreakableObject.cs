using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject m_brokenWallPieces; 

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Anchor"))
    	{
            Instantiate(m_brokenWallPieces, transform.position, transform.rotation);
            Destroy(gameObject); 
        }
    }
}
