using System.IO;
using System;
using System.Collections.Generic;

public static class CsvUtils
{
    private static string m_SavePath {
        get
    {
        string gamePath = @"My Games\A Foxs Tale";
        string myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string fullPath = Path.Combine(myDocPath, gamePath);
            return fullPath;
        }
    }

    private static string m_FullSavePath => Path.Combine(m_SavePath, "SpeedrunDatabase.csv");

    public static void CreateSaveFile()
    {
        // Create directory for the game (if there isn't one already)
        Directory.CreateDirectory(m_SavePath);

        if (File.Exists(m_FullSavePath))
        {
            // File already exists
            return;
        }

        using StreamWriter csv = new StreamWriter(m_FullSavePath);
        csv.WriteLine("name,lv1Time,lv2Time,lv3Time,totalTime,email");
    }

    public static void WriteToFile(SpeedrunProfile profile)
    {
        using StreamWriter csv = new StreamWriter(m_FullSavePath, true);
        csv.WriteLine($"{profile.PlayerName},{profile.Lv1Time},{profile.Lv2Time},{profile.Lv3Time},{profile.TotalTime},{profile.PlayerEmail}");
    }
}
