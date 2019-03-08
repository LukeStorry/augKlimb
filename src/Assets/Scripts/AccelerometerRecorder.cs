using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccelerometerRecorder : MonoBehaviour {

    public Text timerText;
    private float startTime = 0;

    // Use this for initialization
    void Start () {
        timerText.text = "ready";
    }
	
	// Update is called once per frame
	void Update () {
        if (startTime != 0)
        {
            timerText.text = (Time.time-startTime).ToString("#.00");
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
    }

    public void StopTimer()
    {
        startTime = 0;
    }

}
