using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lilypad : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {

        Debug.Log("Triggering");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            transform.position += new Vector3(0, 0.01f, 0);
        }
    }
}
