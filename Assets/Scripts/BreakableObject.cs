using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject m_brokenWallPieces; 

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Anchor"))
    	{
            Instantiate(m_brokenWallPieces, transform.position, transform.rotation);
            Destroy(gameObject); 
        }
    }
}
