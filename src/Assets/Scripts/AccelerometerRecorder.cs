using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AccelerometerRecorder : MonoBehaviour
{
    public Text timerText;
    public Button startButton, stopButton;

    private Int64 startTime;
    private List<Int64> times;
    private List<float> accs;
    private bool running = false;
    private Color buttonReadyColour, buttonNotReadyColour;

    void Start()
    {
        startButton.onClick.AddListener(StartRecord);
        stopButton.onClick.AddListener(StopRecord);
        timerText.text = "ready";
        buttonReadyColour = startButton.GetComponent<Image>().color;
        buttonNotReadyColour = stopButton.GetComponent<Image>().color;
    }

    void FixedUpdate()
    {
        if (running)
        {
            accs.Add(new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z).magnitude);
            times.Add(DateTime.Now.Ticks);
            timerText.text = TimerString();
        }
    }

    private string TimerString()
    {
        return ((DateTime.Now.Ticks - startTime) / 10000000.0).ToString("#0.00");
    }

    public void StartRecord()
    {
        if (running) return;
        running = true;
        Debug.Log("Started");

        startTime = DateTime.Now.Ticks;
        times = new List<Int64>();
        accs = new List<float>();
        startButton.GetComponent<Image>().color = buttonNotReadyColour;
        stopButton.GetComponent<Image>().color = buttonReadyColour;
    }

    public void StopRecord()
    {
        if (!running) return;
        running = false;
        Debug.Log("Stopped");

        string infoText = "Time: " + TimerString() + "\nSmooth Rating: " + CalcSmoothness(accs).ToString("#0.00");
        timerText.text = infoText;

        SaveLists(infoText.Replace("\n", ",  "));

        startButton.GetComponent<Image>().color = buttonReadyColour;
        stopButton.GetComponent<Image>().color = buttonNotReadyColour;

    }

    // Saves the accs and times lists as a timestamped csv file, with the info tex as the first line
    private void SaveLists(string infoText)
    {
        string csvString = infoText + ",\ntimes,accelerations\n";
        for (int i = 0; i < times.Count; i++)
        {
            csvString += times[i].ToString() + "," + accs[i].ToString("#0.000") + "\n";
        }

        string folderPath = Path.Combine(Application.persistentDataPath, "accelerometer");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string filename = "acc_" + DateTime.Now.ToString("yyMMdd-HHmmss") + ".csv";
        File.WriteAllText(Path.Combine(folderPath, filename), csvString);
        Debug.Log(filename + " written to " + folderPath);
    }


    public static float CalcSmoothness(List<float> accs)
    {
        float avg = 0;
        int count = 0;
        foreach (float n in accs)
        {
            avg += n;
            count += 1;
        }
        avg /= count;

        float totalSquaredDiff = 0;
        foreach (float n in accs)
        {
            totalSquaredDiff += Mathf.Pow(n - avg, 2);
        }
        return totalSquaredDiff;
    }
}
