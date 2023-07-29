using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlHUD : MonoBehaviour
{
    [SerializeField]
    private Canvas m_Canvas;

    private void OnControlHUD()
    {
        m_Canvas = GameObject.Find("ControlHUDCanvas").GetComponent<Canvas>();
        m_Canvas.enabled = !m_Canvas.enabled;

    }
}