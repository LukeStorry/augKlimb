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
    public GameObject lineObj;

    private int dataItemHeight = 270;
    private int lineWidth = 5;

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

        public DataPoint(long time, float acc)
        {
            this.time = time;
            this.acc = acc;
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
        // Parse incoming Dataset
        List<DataPoint> data = parseData(dataString);
     
        // Find dataset max & min
        long maxTime = long.MinValue;
        long minTime = long.MaxValue;
        float maxAcc = float.MinValue;
        float minAcc = float.MaxValue;
        foreach (DataPoint dataPoint in data)
        {
            maxTime = (dataPoint.time > maxTime) ? dataPoint.time : maxTime;
            minTime = (dataPoint.time < minTime) ? dataPoint.time : minTime;
            maxAcc = (dataPoint.acc > maxAcc) ? dataPoint.acc : maxAcc;
            minAcc = (dataPoint.acc < minAcc) ? dataPoint.acc : minAcc;
        }
        // Find size of graphContainer (with 10pt padding)
        float graphWidth = -10 + graphContainer.GetComponent<RectTransform>().rect.width;
        float graphHeight = -10 + graphContainer.GetComponent<RectTransform>().rect.height;

        // Calculate mapping values (adding slight offsets to prevent zero edge-case)
        float xMultiplier = graphWidth / (maxTime - minTime + 0.0001f);
        float yMultiplier = graphHeight / (maxAcc - minAcc + 0.0001f);

        // Loop through datapoints adding a line to the previous point, if exists
        Vector2 previous = Vector2.negativeInfinity;
        foreach (DataPoint dataPoint in data)
        {
            float x = (dataPoint.time - minTime) * xMultiplier - graphWidth / 2;
            float y = (dataPoint.acc - minAcc) * yMultiplier - graphHeight / 2;
            Vector2 coord = new Vector2(x, y);

            if (previous != Vector2.negativeInfinity)
            {
                //GameObject point = Instantiate(dot, graphContainer.transform);
                //point.GetComponent<RectTransform>().localPosition = coord;

                GameObject line = Instantiate(lineObj, graphContainer.transform);
                line.GetComponent<RectTransform>().localPosition = coord;

                float length = Vector2.Distance(coord, previous);
                line.GetComponent<RectTransform>().sizeDelta = new Vector2(2 + length, lineWidth);

                float angle = Mathf.Atan2((previous - coord).y, (previous - coord).x) * Mathf.Rad2Deg;
                line.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, angle);
            }
            previous = coord;

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
