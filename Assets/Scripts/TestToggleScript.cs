using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestToggleScript : MonoBehaviour, IToggle
{
    public void Toggle()
    {

    }

    public float GetResetTime()
    {
        return 3;
    }
}
