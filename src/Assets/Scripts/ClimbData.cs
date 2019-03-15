using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClimbData
{
    public float smoothness = 0;
    public List<DataPoint> accelerometer;
    public float TimeTaken
    {
        get { return (accelerometer[accelerometer.Count - 1].time - accelerometer[0].time) / 10000000.0f; }
    }

    public DateTime Date
    {
        get { return new DateTime(accelerometer[0].time); }
    }
    public string InfoText
    {
        get { return "Time: " + TimeTaken.ToString("#0.0") + "\n Smoothness: " + smoothness.ToString("#0.0"); }
    }


    public ClimbData(List<DataPoint> accelerometer)
    {
        this.accelerometer = accelerometer;
        this.smoothness = CalcSmoothness(accelerometer);
        Debug.Log("ClimbData Created");
    }

    // Removes data after the cut-off point, given as 0-1
    public void Crop(float cut)
    {
        if (TimeTaken < 7 || cut < 0.5) return;

        Debug.Log("Cropping climb at " + cut.ToString());
        float newLength = cut * accelerometer.Count;
        accelerometer = accelerometer.GetRange(0, (int) newLength);
        smoothness = CalcSmoothness(accelerometer);
        FileHandler.SaveClimb(this);
    } 
    
    private static float CalcSmoothness(List<DataPoint> data)
    {
        float avg = 0;
        int count = 0;
        foreach (DataPoint n in data)
        {
            avg += n.acc;
            count += 1;
        }
        avg /= count;

        float totalSquaredDiff = 0;
        foreach (DataPoint n in data)
        {
            totalSquaredDiff += Mathf.Pow(n.acc - avg, 2);
        }
        Debug.Log("Smoothness Calculated: " + totalSquaredDiff.ToString("0.000"));
        return totalSquaredDiff;
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