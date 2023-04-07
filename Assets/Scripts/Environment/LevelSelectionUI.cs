using UnityEditor;
using UnityEngine;

public class LevelSelectionUI : MonoBehaviour
{
	[SerializeField]
	private LevelSelector m_levelSelectorPrefab;

    private void Awake()
    {
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
			EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
			if (scene.enabled)
            {
				LevelSelector levelSelector = Instantiate(m_levelSelectorPrefab, gameObject.transform);
				levelSelector.Initialize(i, System.IO.Path.GetFileNameWithoutExtension(scene.path));
			}
        }
	}
}
