using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spike : MonoBehaviour
{
    private CloseOrOpenCircle m_HoleTransition;

    private static bool S_Collided = false; //prevents multiple collision events spamming

    private void Start()
    {
        S_Collided = false;
        if (GameObject.Find("HoleTransition") == null)
        {
            Debug.Log("Can't find the LevelTransitioner prefab in the scene. Ask Sach if help is needed.");
        }
        else
        {
            m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
            m_HoleTransition.OnShrinkComplete += OnShrinkCompleteCallback;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && S_Collided == false)
        {
            S_Collided = true;
            if(m_HoleTransition != null)
            {
                StartCoroutine(m_HoleTransition.ShrinkParentObject());
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
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
