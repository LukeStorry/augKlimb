using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;



public class AccelerometerViewer : MonoBehaviour
{
    public Text fileListTextBox;
    public GameObject scrollContent;
    public GameObject dataItem;
    public int dataItemHeight = 200;
    public int padding = 5;

    void Start () {
        string filenames = "";
        var fileInfo = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "accelerometer")).GetFiles();
        foreach (var file in fileInfo) {
            filenames += file.Name + "\n";
        }
        fileListTextBox.text = filenames;

        GameObject one = Instantiate(dataItem, new Vector3(100, 100, 1), Quaternion.identity, scrollContent.transform);

        GameObject two = Instantiate(dataItem, new Vector3(0, 0, 0), Quaternion.identity, scrollContent.transform);

    }



}
