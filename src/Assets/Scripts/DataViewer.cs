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

    struct DataPoint
    {
        public long time; // DateTime Ticks
        public float acc; // Accelerometer data

        public DataPoint(string dataString)
        {
            string[] splitString = dataString.Split(',');
            this.time = long.Parse(splitString[0]);
            this.acc = float.Parse(splitString[1]);
        }
    }

    void Start() {
        scrollContent.transform.Find("DataItem").gameObject.SetActive(false);

        FileInfo[] files = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "accelerometer")).GetFiles("*.csv");
        Array.Reverse(files);
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


    private void CreateGraph(GameObject graphContainer, string dataString)
    {
        float graphWidth = -10 + graphContainer.GetComponent<RectTransform>().rect.width;
        float graphHeight = -10 + graphContainer.GetComponent<RectTransform>().rect.height;

        List<DataPoint> data = parseData(dataString);

        // Find Graph edge values for mappings
        long maxTime = long.MinValue, minTime = long.MaxValue;
        float maxAcc = float.MinValue, minAcc = float.MaxValue;
        foreach (DataPoint dataPoint in data)
        {
            maxTime = (dataPoint.time > maxTime) ? dataPoint.time : maxTime;
            minTime = (dataPoint.time < minTime) ? dataPoint.time : minTime;
            maxAcc = (dataPoint.acc > maxAcc) ? dataPoint.acc : maxAcc;
            minAcc = (dataPoint.acc < minAcc) ? dataPoint.acc : minAcc;
        }
        // Calculate mapping values (adding slight offsets to prevent zero edge-case)
        float xMultiplier = graphWidth / (maxTime - minTime + 0.0001f);
        float yMultiplier = graphHeight / (maxAcc - minAcc + 0.0001f);
        foreach (DataPoint dataPoint in data)
        {
            GameObject point = Instantiate(dot, graphContainer.transform);
            float x = (dataPoint.time - minTime) * xMultiplier - graphWidth/2;
            float y = (dataPoint.acc - minAcc) * yMultiplier - graphHeight / 2;
            point.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
        }

      
    }

    private static List<DataPoint> parseData(string csv)
    {
        List<DataPoint> parsedData = new List<DataPoint>();
        foreach(string line in csv.Split('\n'))
        {
            if (line == "") continue;
            parsedData.Add(new DataPoint(line));
        }
        return parsedData;
    }
    

}
