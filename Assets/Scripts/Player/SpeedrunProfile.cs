public struct SpeedrunProfile
{
    // All times are in ms
    public string PlayerName;
    public float Lv1Time;
    public float Lv2Time;
    public float Lv3Time;
    public float TotalTime;
    public string PlayerEmail;

    // Player name is "YOU" initially so that they'll see where they are on the leaderboard later before inputting their own name
    public SpeedrunProfile(string playerName = "YOU", float lv1Time = 0, float lv2Time = 0, float lv3Time = 0, float totalTime = 0, string playerEmail = "")
    {
        PlayerName = playerName;
        Lv1Time = lv1Time;
        Lv2Time = lv2Time;
        Lv3Time = lv3Time;
        TotalTime = totalTime;
        PlayerEmail = playerEmail;
    }
}