using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BangBangBang : MonoBehaviour
{
    private Animator m_Animator;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        GetComponent<PlayerInput>().actions["Jump"].started += Continue;
    }

    private void Continue(InputAction.CallbackContext ctx)
    {
        m_Animator.SetTrigger("Continue");
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            SceneManager.LoadScene(1);
        }
    }
}
