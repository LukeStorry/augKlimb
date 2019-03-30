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
    private DateTime vidTime;
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
        gameObject.transform.Find("Explanation").gameObject.SetActive(false);
        if (NativeCamera.IsCameraBusy())
        {
            output.text = ("Camera Busy");
            return;
        }
        vidTime = DateTime.Now + new TimeSpan(0, 0, 4); // TODO can we do better than just "it'll start in 4 seconds"?

        NativeCamera.Permission permission = NativeCamera.RecordVideo(HandleVideo, NativeCamera.Quality.Low);

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

        string filename = vidTime.ToString("yyyyMMdd_HHmmss") + Path.GetExtension(tempPath);
        output.text = "Video Recorded: " + filename;

        NativeGallery.SaveVideoToGallery(tempPath, "augKlimb", filename);

        string newPath = Path.Combine(Path.GetDirectoryName(tempPath), filename);
        File.Move(tempPath, newPath);
        Debug.Log(newPath);

        shareButton.SetActive(true);
        shareButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            new NativeShare().AddFile(newPath).Share();
        });
    }

    private IEnumerator ResetText()
    {
        yield return new WaitForSeconds(3);
        output.text = "";
    }


}
