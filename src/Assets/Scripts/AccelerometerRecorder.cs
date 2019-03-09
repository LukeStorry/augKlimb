using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AccelerometerRecorder : MonoBehaviour
{
    public Text timerText;
    private float startTime = -1;
    private List<float> accs;

    void Start()
    {
        timerText.text = "ready";
    }

    void Update()
    {
        if (startTime != -1)
        {
            accs.Add(new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z).magnitude);
            timerText.text = (Time.time - startTime).ToString("#0.00");
        }
    }


    public void StartRecord()
    {
        Debug.Log("Started");
        startTime = Time.time;
        accs = new List<float>();
    }

    public void StopRecord()
    {
        Debug.Log("Stopped");
        startTime = -1;

        string csvString = "";
        foreach(float acc in accs)
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
    }
}
