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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && S_Collided == false)
        {
            S_Collided = true;
            if(m_HoleTransition != null)
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                StartCoroutine(m_HoleTransition.ShrinkParentObject(currentSceneIndex));
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
