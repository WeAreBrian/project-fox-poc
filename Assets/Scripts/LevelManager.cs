using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance => s_Instance;

	private const string k_StorybookTransitionSceneName = "Storybook";

    private static LevelManager s_Instance;

	private void Awake()
	{
		if (s_Instance == null)
		{
			s_Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void TransitionTo(string sceneName)
	{
		//SceneManager.LoadScene(k_StorybookTransitionSceneName, LoadSceneMode.Additive);
	}
}
