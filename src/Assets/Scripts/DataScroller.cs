using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataScroller : MonoBehaviour
{
    public GameObject scrollContent;
    public GameObject dataItem;

    void Start()
    {
        scrollContent.transform.Find("DataItem").gameObject.SetActive(false);

        List<ClimbData> climbs = FileHandler.LoadClimbs();
        foreach (ClimbData climb in climbs) AddToScroll(climb);
    }

    void AddToScroll(ClimbData climb)
    {
        GameObject item = Instantiate(dataItem, scrollContent.transform);

        item.transform.Find("Title").GetComponent<Text>().text = climb.Date.ToString("F", null);
        item.transform.Find("Details").GetComponent<Text>().text = climb.InfoText.Replace("\n", ", ");
        item.GetComponent<Button>().onClick.AddListener(delegate { SelectClimb(climb); });
        GraphDrawer.Draw(item.transform.Find("GraphContainer").gameObject, climb.accelerometer);
    }

    void SelectClimb(ClimbData climb)
    {
        SceneInfo.currentClimb = climb;
        SceneManager.LoadScene("ViewClimb");
    }
}
