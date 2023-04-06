using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateMachine : MonoBehaviour
{

    private Animator m_Animator;
    private Grounded m_Grounded;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Grounded = GetComponent<Grounded>();
    }


    void onMove()
    {

    }

    void onJump()
    {

    }

    void onClimb()
    {

    }

    void onMount()
    {

    }

    void onTug()
    {

    }

    void onAnchorInteract()
    {

    }

    void onAim()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
