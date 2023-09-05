public static class TimeFormatter
{
    public static string Milliseconds(float timeMillisecond)
    {
        int minutes = (int)timeMillisecond / 60;
        int seconds = (int)timeMillisecond % 60;
        int milliseconds = (int)(timeMillisecond * 1000) % 1000;

        var timeText = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

        return timeText;
    }
}
