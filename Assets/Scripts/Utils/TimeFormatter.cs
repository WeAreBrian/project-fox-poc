public static class TimeFormatter
{
    public static string Milliseconds(float timeMillisecond)
    {
        int minutes = (int)timeMillisecond / 60;
        int seconds = (int)timeMillisecond % 60;
        int milliseconds = (int)(timeMillisecond * 100) % 100;

        var timeText = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

        return timeText;
    }
}
