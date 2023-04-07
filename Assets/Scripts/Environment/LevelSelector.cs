using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
	[SerializeField]
	private TMP_Text m_buttonText;
	private int m_levelIndex;

	public void Initialize(int levelIndex, string buttonText)
    {
		Debug.Log(buttonText);
		m_levelIndex = levelIndex;
		m_buttonText.text = buttonText;
	}

	public void OpenScene()
	{
		SceneManager.LoadScene(m_levelIndex);
	}
}
