using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
	[SerializeField]
	private TMP_Text m_buttonText;

	[SerializeField]
	private int m_levelIndex;

    private CloseOrOpenCircle m_HoleTransition;


    public void Initialize(int levelIndex, string buttonText)
    {
		m_levelIndex = levelIndex;
		m_buttonText.text = buttonText;

        if (GameObject.Find("HoleTransition") == null)
        {
            Debug.Log("Can't find the LevelTransitioner prefab in the scene. Ask Sach if help is needed.");
        }
        else
        {
            m_HoleTransition = GameObject.Find("HoleTransition").GetComponent<CloseOrOpenCircle>();
        }
    }

	public void OpenScene()
	{
        //Get the scene name from the build index:
        string m_SceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(m_levelIndex));
        StartCoroutine(m_HoleTransition.ShrinkParentObject(m_SceneName));
	}
}
