using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Videos : MonoBehaviour
{

    private Text output;

    void Start()
    {
        output = gameObject.transform.Find("Text").GetComponent<Text>();
        gameObject.transform.Find("Record Button").GetComponent<Button>().onClick.AddListener(RecordVideo);
        //RecordVideo();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);
    }

    // Records a video, using https://github.com/yasirkula/UnityNativeCamera
    private void RecordVideo()
    {
        DateTime vidTime = DateTime.Now; // TODO edit to take time from video recorder, as this doesnt start the recrod!


        if (NativeCamera.IsCameraBusy())
            output.text = ("Camera Busy");

        NativeCamera.Permission permission = NativeCamera.RecordVideo(HandleVideo, NativeCamera.Quality.Low);

        Debug.Log("Permission result: " + permission);
    }

    private void HandleVideo(string path)
    {
        DateTime vidTime = DateTime.Now.Subtract(new TimeSpan(0, 0, 2, (int)NativeCamera.GetVideoProperties(path).duration));
        Debug.Log("Video time: " + vidTime + " path: " + path);

        if (path != null)
        {
            string filepath = FileHandler.CopyVideo(path, vidTime);
            output.text = "Video Recorded: " + Path.GetFileName(filepath);
        }
        else
        {
            output.text = "Recording failed";
        }

        StartCoroutine(ResetText());
    }

    private IEnumerator ResetText()
    {
        yield return new WaitForSeconds(3);
        output.text = "";
    }


}
