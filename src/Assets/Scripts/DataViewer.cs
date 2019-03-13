using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;



public class DataViewer : MonoBehaviour
{
    public GameObject scrollContent;
    public GameObject dataItem;
    public GameObject dot;

    private int dataItemHeight = 270;

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
        string fileContents = File.ReadAllText(file.FullName);
        string[] splitData = fileContents.Split(new string[] { ",\ntimes,accelerations\n" }, System.StringSplitOptions.None);

        GameObject item = Instantiate(dataItem, scrollContent.transform);

        item.transform.Find("Title").GetComponent<Text>().text = parseFileName(file.FullName);

        CreateGraph(item.transform.Find("GraphContainer").gameObject, splitData[1]);

        item.transform.Find("Details").GetComponent<Text>().text = splitData[0];

    }

    // Converts timestamped filename to pretty datetime string format
    string parseFileName(string filename)
    {
        string dateString = filename.Substring(filename.Length - 17, 13);
        DateTime dt = DateTime.ParseExact(dateString, "yyMMdd-HHmmss", null);
        return dt.ToString("F", null);
    }


    private void CreateGraph(GameObject graphContainer, string data)
    {
        foreach (Vector2 dataPoint in parseData(data))
        {
            GameObject point = Instantiate(dot, graphContainer.transform);
            point.GetComponent<RectTransform>().localPosition = dataPoint;
        }

      
    }

    private static List<Vector2> parseData(string csv)
    {
        Debug.Log(csv);
        List<Vector2> parsedData = new List<Vector2>();
        foreach(string line in csv.Split('\n'))
        {
            if (line == "") continue;
            Debug.Log(line);
            string[] rowData = line.Split(',');
            Debug.Log(rowData[0]);
            parsedData.Add(new Vector2(float.Parse(rowData[0]), float.Parse(rowData[1])));
        }
        return parsedData;
    }

    //private void CreateCircle(Vector2 position, Game)
}
