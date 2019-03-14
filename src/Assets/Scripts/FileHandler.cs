using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileHandler
{
    private static string dateFormat = "yyMMdd-HHmmss";
    private static string climbsFolder = Path.Combine(Application.persistentDataPath, "climbs");


    // Saves the data as a timestamped csv file, with the info text as the first line
    public static void SaveClimb(ClimbData climb)
    {
        if (!Directory.Exists(climbsFolder)) Directory.CreateDirectory(climbsFolder);

        string filename = "climb_" + climb.date.ToString(dateFormat) + ".json";
        string jsonString = JsonUtility.ToJson(climb);

        File.WriteAllText(Path.Combine(climbsFolder, filename), jsonString);
        Debug.Log(filename + " written to " + climbsFolder);
    }

    public static List<ClimbData> LoadClimbs()
    {
        List<ClimbData> climbs = new List<ClimbData>();

        FileInfo[] files = new DirectoryInfo(climbsFolder).GetFiles("*.json");
        Array.Reverse(files);
        foreach (FileInfo file in files)
        {
            string fileContents = File.ReadAllText(file.FullName);
            Debug.Log("File Loaded: " + file.FullName);
            climbs.Add(JsonUtility.FromJson<ClimbData>(fileContents));
        }
        return climbs;
    }
}
