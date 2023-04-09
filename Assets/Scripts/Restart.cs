using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// will possibly convert this script into a singleton - potentially merging with return to level select
public class Restart : MonoBehaviour
{
    private void OnReset() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
 
}

