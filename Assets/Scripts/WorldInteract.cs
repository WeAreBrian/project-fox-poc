using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInteract : MonoBehaviour
{
    public delegate void Trigger();
    public static event Trigger Activated;

    private void OnWorldInteract()
    {
        Activated?.Invoke();
    }
}
