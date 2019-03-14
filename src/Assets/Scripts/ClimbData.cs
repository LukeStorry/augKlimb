using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbData
{
    public DateTime date;
    public string InfoText
    {
        get { return "Time: " + timeTaken.ToString("#0.0") + "\n Smoothness: " + smoothness.ToString("#0.0"); }
    }
    public List<DataPoint> accelerometer;
    public float timeTaken;
    public float smoothness;


    public ClimbData(List<DataPoint> accelerometer, float timeTaken)
    {
        this.date = new DateTime(accelerometer[0].time);
        this.accelerometer = accelerometer;
        this.timeTaken = timeTaken;
        this.smoothness = CalcSmoothness(accelerometer);
        Debug.Log("ClimbData Created");
    }

    public ClimbData(List<DataPoint> accelerometer, float timeTaken, float smoothness)
    {
        this.date = new DateTime(accelerometer[0].time);
        this.accelerometer = accelerometer;
        this.timeTaken = timeTaken;
        this.smoothness = smoothness;
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
        Debug.Log(avg);
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