using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

public class FileHandler
{
    public static string climbsFolder = Path.Combine(Application.persistentDataPath, "climbs");
    public static string vidsFolder = Path.Combine(Application.persistentDataPath, "vids");
    private static string vidDateFormat = "yyyyMMdd_HHmmss";

    // Calculates a timestamped filepath for a climb
    public static string ClimbPath(ClimbData climb)
    {
        return Path.Combine(climbsFolder, "climb_" + climb.Date.ToString("yyMMdd-HHmmss") + ".txt");
    }

    // Saves the climb, both serialized to file and to the PI cache
    public static void SaveClimb(ClimbData climb)
    {
        if (!Directory.Exists(climbsFolder)) Directory.CreateDirectory(climbsFolder);

        string jsonString = JsonUtility.ToJson(climb);
        File.WriteAllText(ClimbPath(climb), jsonString);
        Debug.Log(ClimbPath(climb) + " written.");
        if (!PersistentInfo.Climbs.Contains(climb)) PersistentInfo.Climbs.Insert(0, climb);
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
                climbs.Add(LoadClimb(file.FullName));
            }
            catch (Exception e) { Debug.LogException(e); }
        }
        return climbs;
    }


    // Loads & returns a single climb given a filepath Throws an exception if the unSerialization fails.
    public static ClimbData LoadClimb(string filepath, bool tryMatch = false)
    {
        string fileContents = File.ReadAllText(filepath);
        ClimbData climb = JsonUtility.FromJson<ClimbData>(fileContents);

        if (climb == null) throw new Exception("Climb File not Valid.");
        if (tryMatch)
        {
            foreach (FileInfo file in new DirectoryInfo(vidsFolder).GetFiles("*"))
            {
                if (climb.TryAttachingVideo(file.FullName, CalcVidTime(file.FullName)))
                    Debug.Log("Climb matched with" + climb.video);
            }
            if (climb.video == "")
                Debug.Log("No match found");
        }
        Debug.Log("Climb Loaded: " + climb.accelerometer.Count + " datapoints, from file:  " + filepath);
        return climb;
    }


    // Calculates the video time by parsing the filename
    private static DateTime CalcVidTime(string path)
    {
        DateTime result;
        string filename = Path.GetFileNameWithoutExtension(path);
        Debug.Log("attempt to parse: " + filename);

        if (DateTime.TryParse(filename, out result))
        {
            Debug.Log("Parsed Video Filename: " + result);
            return result;
        }

        string dateString = filename.Substring(filename.Length - 15, 15);
        if (DateTime.TryParseExact(dateString, vidDateFormat, null, DateTimeStyles.None, out result))
        {
            Debug.Log("Exact-parsed Video Filename: " + result);
            return result;
        }

        return File.GetCreationTime(path);
    }


    // Attempts to match a video file to a climb file, then either attaches a copy or throws exception.
    public static string ImportVideo(string oldPath)
    {
        DateTime vidTime = CalcVidTime(oldPath);
        string internalPath = CopyVideo(oldPath, vidTime);

        foreach (ClimbData climb in PersistentInfo.Climbs)
        {
            if (climb.TryAttachingVideo(internalPath, vidTime))
            {
                SaveClimb(climb);
                Debug.Log(vidTime.ToString("F", null) + " matched with climb: " + climb.Date.ToString("F", null));
                return "Matched with climb: " + climb.Date.ToString("F", null);
            }
        }

        return "Couldn't find a matching climb for " + vidTime.ToString("F", null);
    }


    // Copies the video file to PersistentStorage, timestamps the filename, and returns the new filepath
    public static string CopyVideo(string oldPath, DateTime time)
    {
        if (!Directory.Exists(vidsFolder)) Directory.CreateDirectory(vidsFolder);
        string filename = time.ToString(vidDateFormat) + Path.GetExtension(oldPath);
        string newPath = Path.Combine(vidsFolder, filename);
        File.Copy(oldPath, newPath);
        return newPath;
    }

    // Deletes a climb, both from file and from the cache
    public static void RemoveClimb(ClimbData climb)
    {
        try
        {
            File.Delete(climb.video);
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
