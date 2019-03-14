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



    void Start()
    {
        scrollContent.transform.Find("DataItem").gameObject.SetActive(false);

        List<ClimbData> climbs = FileHandler.LoadClimbs();
        foreach (ClimbData climb in climbs)
        {
            AddToScroll(climb);
        }
        scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, dataItemHeight * climbs.Count);
    }

    void AddToScroll(ClimbData climb)
    {

        GameObject item = Instantiate(dataItem, scrollContent.transform);

        item.transform.Find("Title").GetComponent<Text>().text = climb.date.ToString("F", null);
        item.transform.Find("Details").GetComponent<Text>().text = climb.InfoText.Replace("\n", ", ");
        DrawGraph(item.transform.Find("GraphContainer").gameObject, climb.accelerometer);
    }

    private void DrawGraph(GameObject graphContainer, List<DataPoint> data)
    {
        // Find dataset max & min
        long maxTime = long.MinValue;
        long minTime = long.MaxValue;
        float maxAcc = float.MinValue;
        float minAcc = 0;
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
        Vector2 previous = Vector2.zero;
        foreach (DataPoint dataPoint in data)
        {
            float x = (dataPoint.time - minTime) * xMultiplier - graphWidth / 2;
            float y = (dataPoint.acc - minAcc) * yMultiplier - graphHeight / 2;
            Vector2 coord = new Vector2(x, y);

            if (previous != Vector2.zero)
            {
                //GameObject point = Instantiate(dot, graphContainer.transform);
                //point.GetComponent<RectTransform>().localPosition = coord;

                GameObject line = Instantiate(lineObj, graphContainer.transform);
                line.GetComponent<RectTransform>().localPosition = coord;

                float length = Vector2.Distance(coord, previous);
                line.GetComponent<RectTransform>().sizeDelta = new Vector2(2 + length, lineWidth);

                float angle = Mathf.Atan2((previous - coord).y, (previous - coord).x) * Mathf.Rad2Deg;
                line.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, angle);
            }

            previous = coord;
        }

        // Add Y axis labels
        graphContainer.transform.Find("Min").GetComponent<Text>().text = minAcc.ToString("#0.00");
        graphContainer.transform.Find("Max").GetComponent<Text>().text = maxAcc.ToString("#0.00");

        Debug.Log("Graph Drawn");
    }

}
