using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class VideoRecorder : MonoBehaviour
{
    private Text output;
    private GameObject shareButton;
    void Start()
    {
        output = gameObject.transform.Find("Text").GetComponent<Text>();

        gameObject.transform.Find("Record Button").GetComponent<Button>().onClick.AddListener(RecordVideo);

        shareButton = gameObject.transform.Find("Share Button").gameObject;
        shareButton.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);
    }

    // Records a video, using https://github.com/yasirkula/UnityNativeCamera
    private void RecordVideo()
    {
        Debug.Log(FileHandler.vidsFolder);
        DateTime vidTime = DateTime.Now; // TODO edit to take time from video recorder, as this doesnt start the recrod!

        if (NativeCamera.IsCameraBusy())
            output.text = ("Camera Busy");

        NativeCamera.Permission permission = NativeCamera.RecordVideo(HandleVideo); //, NativeCamera.Quality.Low

        Debug.Log("Permission result: " + permission);
    }

    private void HandleVideo(string tempPath)
    {
        if (tempPath == null || tempPath == "")
        {
            output.text = "Recording failed";
            StartCoroutine(ResetText());
            return;
        }

        string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + Path.GetExtension(tempPath);

        NativeGallery.SaveVideoToGallery(tempPath, "augKlimb", filename);
        output.text = "Video Recorded: " + filename;

        shareButton.SetActive(true);
        shareButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            new NativeShare().AddFile(tempPath).Share();
        });
    }

    private IEnumerator ResetText()
    {
        yield return new WaitForSeconds(3);
        output.text = "";
    }


}
