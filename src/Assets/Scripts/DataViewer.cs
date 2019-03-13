using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;



public class DataViewer : MonoBehaviour
{
    public GameObject scrollContent;
    public GameObject dataItem;
    public int dataItemHeight = 200;

    void Start() {
        scrollContent.transform.Find("DataItem").gameObject.SetActive(false);

        FileInfo[] files = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "accelerometer")).GetFiles("*.csv");
        foreach (FileInfo file in files) {
            addToScroll(file);
        }
        scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, dataItemHeight * files.Length);
    }

    void addToScroll(FileInfo file)
    {
        string data = File.ReadAllText(file.FullName);
        string[] splitData = data.Split(new string[] { "times,accelerations" }, System.StringSplitOptions.None);

        GameObject item = Instantiate(dataItem, scrollContent.transform);

        item.transform.Find("Title").GetComponent<Text>().text = parseFileName(file.FullName);

        //TODO item.transform.Find("image") = CreateGraph(splitData[1]);

        item.transform.Find("Details").GetComponent<Text>().text = splitData[0];

    }

    // Converts timestamped filename to pretty datetime string format
    string parseFileName(string filename)
    {
        string dateString = filename.Substring(filename.Length - 17, 13);
        DateTime dt = DateTime.ParseExact(dateString, "yyMMdd-HHmmss", null);
        return dt.ToString("F", null);
    }


    GameObject CreateGraph(string data)
    {
        return null;
    }

}
