using System.IO;
using System;
using UnityEngine;

public static class CsvUtils
{
    private static string m_FullSavePath;

    public static void CreateSaveFile()
    {
        // Create directory for the game (if there isn't one already)
        string gamePath = @"My Games\A Foxs Tale";
        string myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string fullPath = Path.Combine(myDocPath, gamePath);
        Directory.CreateDirectory(fullPath);

        // Create the save file at the specified path
        string saveFileName = @"SpeedrunDatabase.csv";
        m_FullSavePath = Path.Combine(fullPath, saveFileName);

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
