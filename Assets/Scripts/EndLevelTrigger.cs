using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
	public string NextLevelSceneName;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			// TODO: Get rid of coupling and use some kind of scriptable object game event system
			LevelManager.Instance.TransitionTo(NextLevelSceneName);
		}
	}
}
