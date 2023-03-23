using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MouseDirectionProcessor : InputProcessor<Vector2>
{
    public override Vector2 Process(Vector2 value, InputControl control)
    {
        var windowSize = new Vector2(Screen.width, Screen.height);

        return (value - windowSize / 2).normalized;
    }

#if UNITY_EDITOR
    static MouseDirectionProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        InputSystem.RegisterProcessor<MouseDirectionProcessor>();
    }
}
