using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spike : MonoBehaviour
{
    CloseOrOpenCircle m_HoleTransition;

    private static bool S_Collided = false; //prevents multiple collision events spamming

    private void Start()
    {
        S_Collided = false;
        m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
        m_HoleTransition.OnShrinkComplete += OnShrinkCompleteCallback;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && S_Collided == false)
        {
            S_Collided = true;
            StartCoroutine(m_HoleTransition.ShrinkParentObject());
        }
    }

    private void OnShrinkCompleteCallback() //This will get spammed for every spike in the level
    {
        Debug.Log("Shrink operation in CloseOrOpenCircle is complete.");
    }

    private void OnDisable()
    {
        m_HoleTransition.OnShrinkComplete -= OnShrinkCompleteCallback;

    }
}
