using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MouseDirectionFromPlayerProcessor : InputProcessor<Vector2>
{
	public override Vector2 Process(Vector2 value, InputControl control)
	{
		var player = GameObject.FindWithTag("Player");

		if (player == null)
		{
			return Vector2.zero;
		}

		var screenPosition = (Vector2)Camera.main.WorldToScreenPoint(player.transform.position);

		return (value - screenPosition).normalized;
	}

#if UNITY_EDITOR
	static MouseDirectionFromPlayerProcessor()
	{
		Initialize();
	}
#endif

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Initialize()
	{
		InputSystem.RegisterProcessor<MouseDirectionFromPlayerProcessor>();
	}
}
