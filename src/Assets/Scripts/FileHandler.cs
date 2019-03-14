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
        string csvString = climb.timeTaken.ToString("#0.0") + "," + climb.smoothness.ToString("#0.0") + "\naccelerations:,\n";
        foreach (DataPoint p in climb.accelerometer)
        {
            csvString += p.time.ToString() + "," + p.acc.ToString("#0.000") + "\n";
        }

        if (!Directory.Exists(climbsFolder)) Directory.CreateDirectory(climbsFolder);

        string filename = "climb_" + climb.date.ToString(dateFormat) + ".csv";
        File.WriteAllText(Path.Combine(climbsFolder, filename), csvString);
        Debug.Log(filename + " written to " + climbsFolder);
    }

    public static List<ClimbData> LoadClimbs()
    {
        List<ClimbData> climbs = new List<ClimbData>();

        FileInfo[] files = new DirectoryInfo(climbsFolder).GetFiles("*.csv");
        Array.Reverse(files);
        foreach (FileInfo file in files)
        {
            climbs.Add(ParseClimbFile(file));
        }
        return climbs;
    }

    private static ClimbData ParseClimbFile(FileInfo file)
    {
        string fileContents = File.ReadAllText(file.FullName);
        string[] splitData = fileContents.Split(new string[] { "\naccelerations:,\n" }, System.StringSplitOptions.None);
        string[] timeAndSmooth = splitData[0].Split(',');

        Debug.Log("FileLoaded:" + file.FullName);

        return new ClimbData(ParseDataPoints(splitData[1]), float.Parse(timeAndSmooth[0]), float.Parse(timeAndSmooth[1]));
    }


    // Converts a csv string to a list of DataPoint objects
    private static List<DataPoint> ParseDataPoints(string csv)
    {
        List<DataPoint> parsedData = new List<DataPoint>();
        foreach (string line in csv.Split('\n'))
        {
            if (line == "") continue;
            parsedData.Add(new DataPoint(line));
        }
        return parsedData;
    }
}
