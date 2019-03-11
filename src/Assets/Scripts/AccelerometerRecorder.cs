using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AccelerometerRecorder : MonoBehaviour
{
    public Text timerText;
    public Button startButton, stopButton;

    private float startTime = -1;
    private List<float> accs;
    private bool running = false;
    public Color buttonReadyColour, buttonNotReadyColour;

    void Start()
    {
        startButton.onClick.AddListener(StartRecord);
        stopButton.onClick.AddListener(StopRecord);
        timerText.text = "ready";
        buttonReadyColour = startButton.GetComponent<Image>().color;
        buttonNotReadyColour = stopButton.GetComponent<Image>().color;
    }

    void Update()
    {
        if (running)
        {
            accs.Add(new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z).magnitude);
            timerText.text = (Time.time - startTime).ToString("#0.00");
        }
    }


    public void StartRecord()
    {
        if (running) return;
        running = true;
        Debug.Log("Started");

        startTime = Time.time;
        accs = new List<float>();
        startButton.GetComponent<Image>().color = buttonNotReadyColour;
        stopButton.GetComponent<Image>().color = buttonReadyColour;
    }

    public void StopRecord()
    {
        if (!running) return;
        running = false;
        Debug.Log("Stopped");
        timerText.text = "Time: " + (Time.time - startTime).ToString("#0.00")
            + "\nSmoothness: " + CalcSmoothness().ToString("#0.00");

        string csvString = "";
        foreach (float acc in accs)
        {
            csvString += acc.ToString("#0.000") + ",";
        }

        string folderPath = Path.Combine(Application.persistentDataPath, "accelerometer");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string filename = "acc_" + DateTime.Now.ToString("yyMMdd-HHmmss") + ".txt";
        File.WriteAllText(Path.Combine(folderPath, filename), csvString);
        Debug.Log(filename + " written to " + folderPath);

        startButton.GetComponent<Image>().color = buttonReadyColour;
        stopButton.GetComponent<Image>().color = buttonNotReadyColour;

    }


    public float CalcSmoothness()
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
