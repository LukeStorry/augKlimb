using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

public class FileHandler
{
    public static string climbsFolder = Path.Combine(Application.persistentDataPath, "climbs");
    public static string vidsFolder = Path.Combine(Application.persistentDataPath, "videos");
    private static string vidDateFormat = "yyyyMMdd_HHmmssfff";

    // Calculates a timestamped filepath for a climb
    public static string ClimbPath(ClimbData climb)
    {
        return Path.Combine(climbsFolder, "climb_" + climb.Date.ToString("yyMMdd-HHmmss") + ".txt");
    }

    // Saves the climb serialized to file, and optionally adds to the PI cache if not already there
    public static void SaveClimb(ClimbData climb, bool addToCache)
    {
        if (!Directory.Exists(climbsFolder)) Directory.CreateDirectory(climbsFolder);

        string jsonString = JsonUtility.ToJson(climb);
        File.WriteAllText(ClimbPath(climb), jsonString);
        Debug.Log(ClimbPath(climb) + " written.");

        if (addToCache) PersistentInfo.Climbs.Insert(0, climb);
    }


    // Parses and returns all Climb files in data folder
    public static List<ClimbData> LoadClimbs()
    {
        List<ClimbData> climbs = new List<ClimbData>();
        if (!Directory.Exists(climbsFolder)) return climbs;
        FileInfo[] files = new DirectoryInfo(climbsFolder).GetFiles("*.txt");
        Array.Reverse(files);
        foreach (FileInfo file in files)
        {
            try
            {
                climbs.Add(LoadClimb(file.FullName, false));
            }
            catch (Exception e) { Debug.LogException(e); }
        }
        return climbs;
    }


    // Loads & returns a single climb given a filepath Throws an exception if the unSerialization fails. copy parameter also saves to app's storage&cache
    public static ClimbData LoadClimb(string filepath, bool copy = false)
    {
        string fileContents = File.ReadAllText(filepath);
        ClimbData climb = JsonUtility.FromJson<ClimbData>(fileContents);

        if (climb == null) throw new Exception("Climb File not Valid.");

        Debug.Log("Climb Loaded: " + climb.accelerometer.Count + " datapoints, from file:  " + filepath);

        if (copy)
            SaveClimb(climb, true);

        return climb;
    }


    // Calculates the video time by parsing the filename
    public static DateTime CalcVidTime(string path)
    {
        DateTime result;
        string dateString = Path.GetFileNameWithoutExtension(path);

        if (dateString.Contains("20")) {
            int startIndex = dateString.IndexOf('2');
            dateString = dateString.Substring(startIndex, dateString.Length - startIndex);
        }

        Debug.Log("attempt to parse: " + dateString);

        if (DateTime.TryParse(dateString, out result))
        {
            Debug.Log("Parsed Video Filename: " + result);
            return result;
        }

        string[] dateFormats = { "yyyyMMdd_HHmmss", "yyyyMMdd_HHmmssfff", "yyyyMMdd_HHmmss_fff" };
        if (DateTime.TryParseExact(dateString, dateFormats, null, DateTimeStyles.None, out result))
        {
            Debug.Log("Exact-parsed Video Filename: " + result);
            return result;
        }

        return File.GetCreationTime(path);
    }


    // Attempts to match a video file to a climb file, then either attaches a copy or throws exception.
    public static void ImportVideo(string oldPath)
    {
        DateTime vidTime = CalcVidTime(oldPath);
        string vidFilepath = CopyVideo(oldPath, vidTime);

        ClimbData climb = PersistentInfo.CurrentClimb;
        climb.VideoPath = vidFilepath;

        SaveClimb(climb, false);
    }


    // Copies the video file to PersistentStorage, timestamps the filename, and returns the new filepath
    public static string CopyVideo(string oldPath, DateTime time)
    {
        if (!Directory.Exists(vidsFolder)) Directory.CreateDirectory(vidsFolder);

        string filename = time.ToString(vidDateFormat) + Path.GetExtension(oldPath);
        string newPath = Path.Combine(vidsFolder, filename);
        File.Copy(oldPath, newPath, true);
        Debug.Log("Copied video: " + oldPath + " to " + newPath);

        return newPath;
    }


    // Deletes a climb, both from file and from the cache
    public static void RemoveClimb(ClimbData climb)
    {
        try
        {
            File.Delete(climb.VideoPath);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        };

        File.Delete(ClimbPath(climb));
        Debug.Log(ClimbPath(climb) + " deleted.");
        PersistentInfo.Climbs.Remove(climb);
    }
}
