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
    [SerializeField]
    private InputDialog m_ProfileInputDialog;

    private PlayerInputActions m_PlayerInputActions;
    private List<SpeedrunStatTexts> m_SpeedrunStats = new List<SpeedrunStatTexts>();

    private SpeedrunProfile m_PlayerProfile;
    private GameObject playerProfile;
    private int m_PlayerRank;

    private void Awake()
    {
        m_PlayerInputActions = new PlayerInputActions();

        // Change to UI action maps (should be changed back in InputDialog.cs)
        m_PlayerInputActions.Player.Disable();
        m_PlayerInputActions.UI.Enable();
    }

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
        m_PlayerRank = orderedProfiles.IndexOf(m_PlayerProfile) + 1;
        playerProfile = Instantiate(m_ProfilePrefab, m_PlayerProfileSpawn);
        playerProfile.GetComponent<SpeedrunStatTexts>().Initialize(m_PlayerRank, m_PlayerProfile);

        // Show other profiles
        int loopLimit = Mathf.Min(orderedProfiles.Count, 10); // Only show top 10 profiles
        for (int i = 0; i < loopLimit; i++)
        {
            Vector3 position = new Vector3(0, -i * m_ProfileTextPadding, 0);
            GameObject statObject = Instantiate(m_ProfilePrefab, m_TopProfileSpawn);
            statObject.GetComponent<RectTransform>().anchoredPosition = position;

            SpeedrunStatTexts statText = statObject.GetComponent<SpeedrunStatTexts>();
            statText.Initialize(i + 1, orderedProfiles[i]);
            m_SpeedrunStats.Add(statText);
        }

        // Wait 3 seconds before showing dialog
        LeanTween.delayedCall(3.0f, ActivateDialog);
    }

    private void ActivateDialog()
    {
        m_ProfileInputDialog.ActivateDialog();
    }

    public void UpdateProfileName()
    {
        m_PlayerProfile = SaveUtils.GetPlayerData();
        playerProfile.GetComponent<SpeedrunStatTexts>().Initialize(m_PlayerRank, m_PlayerProfile);

        // Also update the name in the leaderboard if the player is top 10
        if (m_PlayerRank < 10)
        {
            m_SpeedrunStats[m_PlayerRank - 1].Initialize(m_PlayerRank, m_PlayerProfile);
        }
    }
}
