using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
	public static bool s_GamePaused = false;
	private VisualElement m_PauseRoot;
	private float m_PreviousTimeScale;   //Incase in future we add any slow-mo effects or whatever than pause menu wont mess with it
	private VisualElement m_LevelSelectRoot;

	private void OnEnable()
	{
		s_GamePaused = false;
        //Get levelselect object
        GameObject m_LevelSelectObject = GameObject.Find("LevelSelectMenu");
		m_LevelSelectRoot = m_LevelSelectObject.GetComponent<UIDocument>().rootVisualElement;
		//enable the component (its disabled by default so doesnt get in the way of the scene view when editting levels
		GetComponent<UIDocument>().enabled = true;
		m_PauseRoot = GetComponent<UIDocument>().rootVisualElement;
		//Hide UI on load
		m_PauseRoot.style.display = DisplayStyle.None;
		//getting all the buttons
		Button m_ResumeButton = m_PauseRoot.Q<Button>("Resume");
		Button m_LevelSelectButton = m_PauseRoot.Q<Button>("LevelSelect");
		Button m_SettingsButton = m_PauseRoot.Q<Button>("Settings");
		Button m_MainMenuButton = m_PauseRoot.Q<Button>("MainMenu");

		//Setting what dem buttons do
		m_ResumeButton.clicked += () => UnpauseGame();
		//Show levelselect ui
		m_LevelSelectButton.clicked += () => m_LevelSelectRoot.style.display = DisplayStyle.Flex;
		m_SettingsButton.clicked += () => Debug.Log("TODO Settings");
		m_MainMenuButton.clicked += () => LoadScene("MainMenu");
	}

	//Note update still runs when game is paused (Timescale = 0) but FixedUpdate does NOT. Input checks in other Update()s still go off so need to check if Pause.s_GamePaused = false for other stuff.
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (s_GamePaused)
			{
				//If currently paused:
				UnpauseGame();
			}
			else
			{
				//If not paused:
				PauseGame();
			}
		}
	}

	private void PauseGame()
	{
		s_GamePaused = true;
		m_PreviousTimeScale = Time.timeScale;	
		Time.timeScale = 0;
		AudioListener.pause = true; //pauses audio
		m_PauseRoot.style.display = DisplayStyle.Flex;
	}

	private void UnpauseGame()
	{
		s_GamePaused = false;
		Time.timeScale = m_PreviousTimeScale;   //Sets timescale to previous timescale (instead of 1) so that incase there was a slowmo effect then it doesnt cancel it.
		AudioListener.pause = false;
		m_PauseRoot.style.display = DisplayStyle.None;
		m_LevelSelectRoot.style.display = DisplayStyle.None;
	}

	private void LoadScene(string scene)
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(scene);
	}
}
