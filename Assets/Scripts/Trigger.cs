using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent trigger;
    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered) 
        { 
            triggered = true;
            trigger.Invoke();
            Destroy(gameObject);
        }
    }
}
