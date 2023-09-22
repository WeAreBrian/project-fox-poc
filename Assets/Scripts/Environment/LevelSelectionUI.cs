using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionUI : MonoBehaviour
{
	[SerializeField]
	private LevelSelector m_levelSelectorPrefab;

    private void Awake()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
		    string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (!scenePath.Contains("LevelSelection") && !scenePath.Contains("LevelEnd"))
            {
                LevelSelector levelSelector = Instantiate(m_levelSelectorPrefab, gameObject.transform);
                levelSelector.Initialize(i, $"Level {i}");
            }
        }
    }
}
