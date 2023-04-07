using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
	[SerializeField]
	private TMP_Text m_buttonText;

	[SerializeField]
	private int m_levelIndex;

	public void Initialize(int levelIndex, string buttonText)
    {
		m_levelIndex = levelIndex;
		m_buttonText.text = buttonText;
	}

	public void OpenScene()
	{
		SceneManager.LoadScene(m_levelIndex);
	}
}
