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
    [SerializeField]
    private GameObject m_SilverTrophy;
    [SerializeField]
    private GameObject m_BronzeTrophy;

    private SpeedrunProfile m_PlayerProfile;

    private void Start()
    {
        // Load player profiles
        m_PlayerProfile = SaveUtils.GetPlayerData();
        List<SpeedrunProfile> orderedProfiles = SaveUtils.LoadProfiles();

        if (orderedProfiles.Count < 2)
        {
            m_SilverTrophy.SetActive(false);
        }
        if (orderedProfiles.Count < 3)
        {
            m_BronzeTrophy.SetActive(false);
        }

        // Show current player profile and their rank
        GameObject playerProfile = Instantiate(m_ProfilePrefab, m_PlayerProfileSpawn);
        playerProfile.GetComponent<SpeedrunStatTexts>().Initialize(orderedProfiles.IndexOf(m_PlayerProfile) + 1, m_PlayerProfile);

        // Show other profiles
        for (int i = 0; i < 10; i++) // Only show top 10 profiles
        {
            if (i >= orderedProfiles.Count)
            {
                break;
            }
            Vector3 position = new Vector3(0, -i * m_ProfileTextPadding, 0);
            GameObject stat = Instantiate(m_ProfilePrefab, m_TopProfileSpawn);
            stat.GetComponent<SpeedrunStatTexts>().Initialize(i + 1, orderedProfiles[i]);
            stat.GetComponent<RectTransform>().anchoredPosition = position;
        }
    }
}
