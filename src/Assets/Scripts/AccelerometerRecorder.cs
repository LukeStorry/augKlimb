using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AccelerometerRecorder : MonoBehaviour
{
    public Text timerText;
    public Button startButton, stopButton;

    private Int64 startTime;
    private List<DataPoint> data;
    private bool running = false;
    private Color buttonReadyColour, buttonNotReadyColour;
    private int countdown;

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
        if (!running) return;

        if (Timer() < 0)
        {
            timerText.text = "starting in " + Mathf.Ceil(-Timer()).ToString();
            if (countdown > -Timer())
            {
                Handheld.Vibrate();
                countdown -= 1;
            }
        }
        else
        {
            timerText.text = Timer().ToString("#0.0");
            float acc = new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z).magnitude;
            data.Add(new DataPoint(DateTime.Now.Ticks, acc));
        }

    }


    public void StartRecord()
    {
        if (running) return;

        running = true;
        Debug.Log("Started");

        countdown = 5;
        startTime = DateTime.Now.Ticks + countdown * 10000000;
        data = new List<DataPoint>();
        startButton.GetComponent<Image>().color = buttonNotReadyColour;
        stopButton.GetComponent<Image>().color = buttonReadyColour;
    }

    public void StopRecord()
    {
        if (!running) return;
        running = false;
        Debug.Log("Stopped");

        if (data.Count > 10)
        {
            ClimbData climb = new ClimbData(data);
            timerText.text = climb.InfoText;

            PersistentInfo.climbs.Insert(0, climb);
            FileHandler.SaveClimb(climb);
        }

        startButton.GetComponent<Image>().color = buttonReadyColour;
        stopButton.GetComponent<Image>().color = buttonNotReadyColour;

    }

    private float Timer()
    {
        return (DateTime.Now.Ticks - startTime) / 10000000.0f;
    }


}
