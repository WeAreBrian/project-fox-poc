using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ProfilePrefab;
    [SerializeField]
    private float m_ProfileTextPadding = 60f;
    [SerializeField]
    private Transform m_TopProfileSpawn;
    [SerializeField]
    private Transform m_PlayerProfileSpawn;

    private SpeedrunProfile m_PlayerProfile;

    void Start()
    {
        m_PlayerProfile = SaveUtils.GetPlayerData();
        GameObject playerProfile = Instantiate(m_ProfilePrefab, m_PlayerProfileSpawn);
        playerProfile.GetComponent<SpeedrunStatTexts>().Initialize(m_PlayerProfile);
        List<SpeedrunProfile> profiles = SaveUtils.LoadProfiles();

        // Only show 10 profiles
        for (int i = 0; i < 10; i++)
        {
            if (i >= profiles.Count)
            {
                break;
            }
            Vector3 position = new Vector3(0, -i * m_ProfileTextPadding, 0);
            GameObject stat = Instantiate(m_ProfilePrefab, m_TopProfileSpawn);
            stat.GetComponent<SpeedrunStatTexts>().Initialize(profiles[i]);
            stat.GetComponent<RectTransform>().anchoredPosition = position;
        }
    }
}
