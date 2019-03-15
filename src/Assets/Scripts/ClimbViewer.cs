using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClimbViewer : MonoBehaviour
{
    public GameObject scrollContent;
    private ClimbData climb;
    void Start()
    {
        climb = PersistentInfo.currentClimb;
        gameObject.transform.Find("Title").GetComponent<Text>().text = climb.Date.ToString("F", null);
        gameObject.transform.Find("Details").GetComponent<Text>().text = climb.InfoText.Replace("\n", ", ");

        GameObject graphContainer = scrollContent.transform.Find("GraphContainer").gameObject;

        Vector2 graphSize = new Vector2(100 * climb.TimeTaken, graphContainer.GetComponent<RectTransform>().rect.height);
        graphContainer.GetComponent<RectTransform>().sizeDelta = graphSize;
        scrollContent.GetComponent<RectTransform>().sizeDelta = graphSize + new Vector2(20, 0);

        GraphDrawer.Draw(graphContainer, climb.accelerometer, includeDots: true);

        gameObject.transform.Find("Crop Button").GetComponent<Button>().onClick.AddListener(Crop);
    }


    void Crop()
    {
        float scrollPosition = gameObject.transform.Find("Scroll View").GetComponent<ScrollRect>().horizontalNormalizedPosition;
        climb.Crop(Mathf.Sqrt(scrollPosition));
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


