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
    public SpeedrunProfile(string playerName, float lv1Time, float lv2Time, float lv3Time, float totalTime, string playerEmail)
    {
        PlayerName = playerName;
        Lv1Time = lv1Time;
        Lv2Time = lv2Time;
        Lv3Time = lv3Time;
        TotalTime = totalTime;
        PlayerEmail = playerEmail;
    }
}