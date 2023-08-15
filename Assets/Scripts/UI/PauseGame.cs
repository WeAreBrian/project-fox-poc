using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseGame : MonoBehaviour
{
    GameObject m_PauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        m_PauseMenu = GameObject.Find("PauseMenu");
        
    }

    private void OnPause()	//uses input system
    {
        m_PauseMenu.GetComponent<Pause>().TryPause();
    }
}
