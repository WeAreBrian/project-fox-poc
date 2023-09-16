using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private Transform m_StatsSpawn;
    [SerializeField]
    private GameObject m_StatPrefab;

    private SpeedrunProfile m_Profile;

    void Start()
    {
        m_Profile = SaveUtils.GetPlayerData();
        GameObject stat = Instantiate(m_StatPrefab, m_StatsSpawn);
        stat.GetComponent<SpeedrunStatTexts>().Initialize(m_Profile);
    }
}
