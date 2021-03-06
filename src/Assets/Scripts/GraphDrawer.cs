﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphDrawer
{
    private GameObject graphContainer;
    private List<DataPoint> data;
    private long minTime, maxTime;
    private float minAcc, maxAcc;
    private float graphWidth, graphHeight;
    private float xMultiplier, yMultiplier;

    public GraphDrawer(GameObject graphContainer, List<DataPoint> data)
    {
        this.graphContainer = graphContainer;
        this.data = data;

        // Find dataset max & min
        minTime = data[0].time;
        maxTime = data[data.Count - 1].time;
        minAcc = 0;
        maxAcc = float.MinValue;
        foreach (DataPoint dataPoint in data)
        {
            maxAcc = (dataPoint.acc > maxAcc) ? dataPoint.acc : maxAcc;
            minAcc = (dataPoint.acc < minAcc) ? dataPoint.acc : minAcc;
        }

        // Find size of graphContainer (with 10pt padding)
        graphWidth = -10 + graphContainer.GetComponent<RectTransform>().rect.width;
        graphHeight = graphContainer.GetComponent<RectTransform>().rect.height;

        // Calculate mapping values (adding slight offsets to prevent zero edge-case)
        xMultiplier = graphWidth / (maxTime - minTime + 0.0001f);
        yMultiplier = (graphHeight - 40) / (maxAcc - minAcc + 0.0001f);
    }

    // Draws a graph of the given data, within the bounds of a given graphContainer's RectTransform
    public void Draw(bool includeDots = false, bool includeSeconds = false)
    {
        if (includeSeconds) DrawSecondsBoxes();
        DrawLines(includeDots);
    }

    private void DrawLines(bool includeDots, float lineWidth = 5)
    {
        GameObject dotObj = Resources.Load("Dot") as GameObject;
        GameObject lineObj = Resources.Load("Line") as GameObject;

        // Loop through datapoints adding a line to the previous point, if exists
        Vector2 previous = Vector2.zero;
        foreach (DataPoint dataPoint in data)
        {
            float x = (dataPoint.time - minTime) * xMultiplier;
            float y = (dataPoint.acc - minAcc) * yMultiplier + 10;
            Vector2 coord = new Vector2(x, y);

            if (previous != Vector2.zero)
            {
                if (includeDots)
                {
                    GameObject dot = Object.Instantiate(dotObj, graphContainer.transform);
                    dot.GetComponent<RectTransform>().localPosition = coord;
                }

                GameObject line = Object.Instantiate(lineObj, graphContainer.transform);
                line.GetComponent<RectTransform>().localPosition = coord;

                float length = Vector2.Distance(coord, previous);
                line.GetComponent<RectTransform>().sizeDelta = new Vector2(2 + length, lineWidth);

                float angle = Mathf.Atan2((previous - coord).y, (previous - coord).x) * Mathf.Rad2Deg;
                line.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, angle);
            }

            previous = coord;
        }

    }

    private void DrawSecondsBoxes()
    {
        GameObject secondsBox = Resources.Load("Seconds Box") as GameObject;
        int indexStartOfSecond = 0;
        float duration = (data[data.Count - 1].time - data[0].time) / 10000000.0f;

        for (int second = 0; second < duration; second++)
        {
            // calculate the smoothness for that second
            int indexEndOfSecond = data.FindIndex(indexStartOfSecond, (x => x.time > (minTime + (second + 1) * 10000000)));
            if (indexEndOfSecond < 0) continue;
            float smoothness = ClimbData.CalcSmoothness(data.GetRange(indexStartOfSecond, indexEndOfSecond - indexStartOfSecond));
            indexStartOfSecond = indexEndOfSecond;

            // create and set the seconds box
            GameObject box = Object.Instantiate(secondsBox, graphContainer.transform);
            box.GetComponent<RectTransform>().localPosition = new Vector2((second * 10000000.0f) * xMultiplier, 0);
            box.GetComponent<RectTransform>().sizeDelta = new Vector2(100, graphHeight);

            box.transform.Find("Second Label").GetComponent<Text>().text = second.ToString()+"s";
            box.transform.Find("Smoothness Label").GetComponent<Text>().text = smoothness.ToString("0.0");
            box.GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Clamp(smoothness, 0, 200) / 300);
        }
    }

    // Draws line at location signified by parameter given as 0-1 
    public static GameObject DrawVerticalLine(GameObject graphContainer, float location, Color color)
    {
        GameObject lineObj = Resources.Load("Line") as GameObject;

        GameObject line = Object.Instantiate(lineObj, graphContainer.transform);
        line.GetComponent<RectTransform>().localPosition = new Vector2(location * graphContainer.GetComponent<RectTransform>().rect.width, -1000);
        line.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 10);
        line.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
        line.GetComponent<Image>().color = color;
        return line;
    }
}
