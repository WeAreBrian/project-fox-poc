using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forcefield : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<AnchorHolder>().DropAnchor();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Surf"))
        {
            collision.gameObject.GetComponent<AnchorHolder>().Boost(collision.contacts[0].normal);
        }
    }
}
