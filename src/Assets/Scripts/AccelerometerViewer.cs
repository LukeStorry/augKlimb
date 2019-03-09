using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;



public class AccelerometerViewer : MonoBehaviour
{
    public Text fileListTextBox;

    void Start () {
        string filenames = "";
        var fileInfo = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "accelerometer")).GetFiles();
        foreach (var file in fileInfo) {
            filenames += file.Name + "\n";
        }
        fileListTextBox.text = filenames;
    }



}
