using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClimbData
{
    public float smoothness;
    public List<DataPoint> accelerometer;
    public float timeTaken
    {
        get { return (accelerometer[accelerometer.Count - 1].time - accelerometer[0].time) / 10000000.0f; }
    }

    public DateTime date
    {
        get { return new DateTime(accelerometer[0].time); }
    }
    public string InfoText
    {
        get { return "Time: " + timeTaken.ToString("#0.0") + "\n Smoothness: " + smoothness.ToString("#0.0"); }
    }


    public ClimbData(List<DataPoint> accelerometer)
    {
        this.accelerometer = accelerometer;
        this.smoothness = CalcSmoothness(accelerometer);
        Debug.Log("ClimbData Created");
    }


    //TODO add "remove from XX time", and update timeTaken & smoothness in there too.

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