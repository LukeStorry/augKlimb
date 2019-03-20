using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Videos : MonoBehaviour {

    void Start() {
        gameObject.transform.Find("Back Button").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene(0); });
        gameObject.transform.Find("Record Button").GetComponent<Button>().onClick.AddListener(delegate { RecordVideo(); });
        RecordVideo();
    }

    // Records a video, using https://github.com/yasirkula/UnityNativeCamera
    private void RecordVideo()
    {
        DateTime vidTime = DateTime.Now; //TODO record only on the exact second?

        if (NativeCamera.IsCameraBusy())
            Debug.Log("Camera Busy");

        NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                FileHandler.CopyVideo(path, vidTime);
            }
        });

        Debug.Log("Permission result: " + permission);
    }


}
