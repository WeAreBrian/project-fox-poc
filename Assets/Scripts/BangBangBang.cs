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
        GetComponent<PlayerInput>().actions["Skip"].started += Skip;
    }

    private void Continue(InputAction.CallbackContext ctx)
    {
        m_Animator.SetTrigger("Continue");
        //if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("End"))
        //{
        //    SceneManager.LoadScene(1);
        //}
    }

    private void Skip(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(3);
    }

    private void OnDestroy()
    {
        GetComponent<PlayerInput>().actions["Jump"].started -= Continue;
        GetComponent<PlayerInput>().actions["Skip"].started -= Skip;
    }
}
