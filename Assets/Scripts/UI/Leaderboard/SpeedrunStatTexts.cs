using TMPro;
using UnityEngine;

public class SpeedrunStatTexts : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Rank;
    [SerializeField]
    private TextMeshProUGUI m_PlayerName;
    [SerializeField]
    private TextMeshProUGUI m_Lv1Time;
    [SerializeField]
    private TextMeshProUGUI m_Lv2Time;
    [SerializeField]
    private TextMeshProUGUI m_Lv3Time;
    [SerializeField]
    private TextMeshProUGUI m_TotalTime;

    public void Initialize(SpeedrunProfile speedrunProfile)
    {
        m_PlayerName.text = speedrunProfile.PlayerName;
        m_Lv1Time.text = TimeFormatter.Milliseconds(speedrunProfile.Lv1Time);
        m_Lv2Time.text = TimeFormatter.Milliseconds(speedrunProfile.Lv2Time);
        m_Lv3Time.text = TimeFormatter.Milliseconds(speedrunProfile.Lv3Time);
        m_TotalTime.text = TimeFormatter.Milliseconds(speedrunProfile.TotalTime);
    }
}
