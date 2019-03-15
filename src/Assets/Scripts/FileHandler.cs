using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileHandler
{
    public static string climbsFolder = Path.Combine(Application.persistentDataPath, "climbs");

    // Saves the climb, both serialized to file and to the PI cache
    public static void SaveClimb(ClimbData climb)
    {
        if (!Directory.Exists(climbsFolder)) Directory.CreateDirectory(climbsFolder);

        string jsonString = JsonUtility.ToJson(climb);
        File.WriteAllText(ClimbPath(climb), jsonString);
        Debug.Log(ClimbPath(climb) + " written.");
        PersistentInfo.Climbs.Insert(0, climb);
    }

    // Calculates a timestamped filepath for a climb
    public static string ClimbPath(ClimbData climb)
    {
        return Path.Combine(climbsFolder, "climb_" + climb.Date.ToString("yyMMdd-HHmmss") + ".txt");
    }

    // Parses and returns all climb files in data folder
    public static List<ClimbData> LoadClimbs()
    {
        List<ClimbData> climbs = new List<ClimbData>();
        FileInfo[] files = new DirectoryInfo(climbsFolder).GetFiles("*.txt");
        Array.Reverse(files);
        foreach (FileInfo file in files)
        {
            try
            {
                climbs.Add(LoadClimb(file.FullName));
            }
            catch (Exception e) { Debug.LogException(e); }
        }
        return climbs;
    }

    // Loads & returns a single climb given a filepath Throws an exception if the unSerialization fails.
    public static ClimbData LoadClimb(string filepath)
    {
        string fileContents = File.ReadAllText(filepath);
        ClimbData climb = JsonUtility.FromJson<ClimbData>(fileContents);

        if (climb == null) throw new Exception("Climb File not Valid.");

        Debug.Log("Climb Loaded: " + climb.accelerometer.Count + " datapoints, from file:  " + filepath);
        return climb;
    }

    // Deletes a climb, both from file and from the cache
    public static void RemoveClimb(ClimbData climb)
    {
        File.Delete(ClimbPath(climb));
        Debug.Log(ClimbPath(climb) + " deleted.");
        PersistentInfo.Climbs.Remove(climb);
    }
}
