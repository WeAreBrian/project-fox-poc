using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spike : MonoBehaviour
{
    CloseOrOpenCircle m_HoleTransition;

    private void Start()
    {
        m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
        m_HoleTransition.OnShrinkComplete += OnShrinkCompleteCallback;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StartCoroutine(m_HoleTransition.ShrinkParentObject());
        }
    }

    private void OnShrinkCompleteCallback()
    {
        //Debug.Log("Shrink operation in CloseOrOpenCircle is complete.");
    }

    private void OnDisable()
    {
        m_HoleTransition.OnShrinkComplete -= OnShrinkCompleteCallback;

    }
}
