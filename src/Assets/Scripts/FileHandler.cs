﻿using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileHandler
{
    public static string climbsFolder = Path.Combine(Application.persistentDataPath, "climbs");

    // Saves the data as a timestamped csv file, with the info text as the first line
    public static void SaveClimb(ClimbData climb)
    {
        if (!Directory.Exists(climbsFolder)) Directory.CreateDirectory(climbsFolder);
        
        string jsonString = JsonUtility.ToJson(climb);
        File.WriteAllText(ClimbPath(climb), jsonString);
        Debug.Log(ClimbPath(climb) + " written.");

        PersistentInfo.climbs.Insert(0, climb);
    }

    public static string ClimbPath(ClimbData climb)
    {
        return Path.Combine(climbsFolder, "climb_" + climb.Date.ToString("yyMMdd-HHmmss") + ".txt");
    }

    public static List<ClimbData> LoadClimbs()
    {
        List<ClimbData> climbs = new List<ClimbData>();

        FileInfo[] files = new DirectoryInfo(climbsFolder).GetFiles("*.txt");
        Array.Reverse(files);
        foreach (FileInfo file in files)
        {
            climbs.Add(LoadClimb(file.FullName));
        }
        return climbs;
    }

    public static ClimbData LoadClimb(string filepath)
    {
        string fileContents = File.ReadAllText(filepath);
        ClimbData climb = JsonUtility.FromJson<ClimbData>(fileContents);

        if (climb == null) throw new Exception("Climb File not Valid.");

        Debug.Log("Climb Loaded: " + climb.accelerometer.Count + " datapoints, from file:  " + filepath);
        PersistentInfo.climbs.Insert(0, climb);
        return climb;
    }

    public static void RemoveClimb(ClimbData climb)
    {
        File.Delete(ClimbPath(climb));
        Debug.Log(ClimbPath(climb) + " deleted.");
        PersistentInfo.climbs.Remove(climb);
    }
}
