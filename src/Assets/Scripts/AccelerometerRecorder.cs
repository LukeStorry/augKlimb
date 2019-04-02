using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AccelerometerRecorder : MonoBehaviour
{
    private Text timerText;
    private Button startButton, stopButton;
    private Int64 startTime;
    private List<DataPoint> data;
    private bool running = false;
    private Color buttonReadyColour, buttonNotReadyColour;
    private int countdown;
    private GameObject viewClimbButton;

    void Start()
    {
        timerText = gameObject.transform.Find("Timer Text").GetComponent<Text>();
        timerText.text = "ready";

        startButton = gameObject.transform.Find("Start Button").GetComponent<Button>();
        startButton.onClick.AddListener(StartButtonPressed);

        stopButton = gameObject.transform.Find("Stop Button").GetComponent<Button>();
        stopButton.onClick.AddListener(StopButtonPressed);

        buttonReadyColour = startButton.GetComponent<Image>().color;
        buttonNotReadyColour = stopButton.GetComponent<Image>().color;

        viewClimbButton = gameObject.transform.Find("View Climb Button").gameObject;
        viewClimbButton.GetComponent<Button>().onClick.AddListener(ViewClimb);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);
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
            timerText.text = "Recording\n\n" + Timer().ToString("#0.0");
            float acc = new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z).magnitude - 1; // -1 offset for gravity
            data.Add(new DataPoint(DateTime.Now.Ticks, acc));
        }

    }


    public void StartButtonPressed()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        gameObject.transform.Find("Explanation").gameObject.SetActive(false);
        if (running) return;

        running = true;
        Debug.Log("Started");

        countdown = 5;
        startTime = DateTime.Now.Ticks + countdown * 10000000;
        data = new List<DataPoint>();
        startButton.GetComponent<Image>().color = buttonNotReadyColour;
        stopButton.GetComponent<Image>().color = buttonReadyColour;
    }


    public void StopButtonPressed()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        if (!running) return;
        running = false;
        Debug.Log("Stopped");

        if (data.Count > 100)
        {
            ClimbData climb = new ClimbData(data);
            FileHandler.SaveClimb(climb, true);
            PersistentInfo.CurrentClimb = climb;
            timerText.text = climb.Details;
            viewClimbButton.SetActive(true);
        }
        else
        {
            timerText.text = "Not enough data recorded, minimum 5 seconds.";
        }

        startButton.GetComponent<Image>().color = buttonReadyColour;
        stopButton.GetComponent<Image>().color = buttonNotReadyColour;
    }


    private void ViewClimb()
    {
        PersistentInfo.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("ViewClimb");
    }

    private float Timer()
    {
        return (DateTime.Now.Ticks - startTime) / 10000000.0f;
    }


}
