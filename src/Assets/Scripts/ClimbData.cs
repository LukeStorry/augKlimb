using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClimbData
{
    [SerializeField]
    private string _title = "";
    [SerializeField]
    private float avgAcceleration;
    [SerializeField]
    private float maxAcceleration;
    [SerializeField]
    private float smoothness;

    [SerializeField]
    private string videoPath = "";
    [SerializeField]
    private float videoOffset = 0; //seconds

    public List<DataPoint> accelerometer;

    public float TimeTaken
    {
        get { return (accelerometer[accelerometer.Count - 1].time - accelerometer[0].time) / 10000000.0f; }
    }

    public DateTime Date
    {
        get { return new DateTime(accelerometer[0].time); }
    }

    public string Title
    {
        get
        {
            if (_title == "")
                return Date.ToString("ddd dd/MM HH:mm");
            else
                return _title;
        }
        set
        {
            _title = value;
            FileHandler.SaveClimb(this, false);
        }
    }

    public string Info
    {
        get
        {
            string output = "Smoothness: " + smoothness.ToString("#0.0"); 
            if (_title == "")
                return output;
            else
                return Date.ToString("dddd dd/MM/yyyy HH:mm.ss") + "                " + output;
        }
    }

    public string Details
    {
        get
        {
            string output = "";

            if (_title != "")
                output += Date.ToString("dddd dd/MM/yyyy HH:mm:ss\n");

            output += "Time Taken: " + TimeTaken.ToString("#0.0") + "\n";
            output += "Average Acceleration: " + avgAcceleration.ToString("#0.0") + "\n";
            output += "Max Acceleration: " + maxAcceleration.ToString("#0.0") + "\n";
            output += "Overall Smoothness: " + smoothness.ToString("#0.0") + "\n";

            return output;
        }
    }


    public string VideoPath
    {
        get { return videoPath; }
        set
        {
            videoPath = value;
            DateTime vidTime = FileHandler.CalcVidTime(videoPath);
            float offset = (float)Date.Subtract(vidTime).TotalMilliseconds / 1000;

            Debug.Log("offset:" + offset);

            if (25 > offset && offset > -TimeTaken)
                videoOffset = offset;
            else
                videoOffset = 0;

           FileHandler.SaveClimb(this, false);
        }
    }

    public float VideoOffset
    {
        get { return videoOffset; }
        set
        {
            videoOffset = value;
            FileHandler.SaveClimb(this, false);
        }
    }

    public ClimbData(List<DataPoint> accelerometer)
    {
        this.accelerometer = accelerometer;
        CalculateAnalytics();
        Debug.Log("ClimbData Created");
    }

    // calculate and cache the analytics for this climb
    private void CalculateAnalytics()
    {
        avgAcceleration = CalcAverageAcceleration(accelerometer);
        maxAcceleration = CalcMaxAcceleration(accelerometer);
        smoothness = CalcSmoothness(accelerometer);
    }

    private static float CalcAverageAcceleration(List<DataPoint> data)
    {
        float sum = 0;
        int count = 0;
        foreach (DataPoint n in data)
        {
            sum += n.acc;
            count += 1;
        }
        return sum / count;
    }

    private static float CalcMaxAcceleration(List<DataPoint> data)
    {
        float max = float.MinValue;
        foreach (DataPoint dataPoint in data)
        {
            max = (dataPoint.acc > max) ? dataPoint.acc : max;
        }
        return max;
    }

    public static float CalcSmoothness(List<DataPoint> data)
    {
        float totalSquaredDiff = 0;
        float avgAcceleration = CalcMaxAcceleration(data);
        foreach (DataPoint n in data)
        {
            totalSquaredDiff += Mathf.Pow(n.acc - avgAcceleration, 2);
        }
        Debug.Log("Smoothness Calculated: " + totalSquaredDiff.ToString("0.000"));
        return totalSquaredDiff;
    }


    // Removes data after the cut-off point, given as 0-1
    public void Crop(float cut)
    {
        Debug.Log("Cropping climb at " + cut.ToString());
        float newLength = cut * accelerometer.Count;
        accelerometer = accelerometer.GetRange(0, (int)newLength);
        CalculateAnalytics();
        FileHandler.SaveClimb(this, false);
    }
}



[Serializable]
public struct DataPoint
{
    public long time; // DateTime Ticks
    public float acc; // Accelerometer data

    public DataPoint(string dataString)
    {
        string[] splitString = dataString.Split(',');
        this.time = long.Parse(splitString[0]);
        this.acc = float.Parse(splitString[1]);
    }

    public DataPoint(long time, float acc)
    {
        this.time = time;
        this.acc = acc;
    }
}