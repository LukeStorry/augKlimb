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
        PersistentInfo.previousScene = SceneManager.GetActiveScene().name;
        scrollContent.transform.Find("DataItem").gameObject.SetActive(false);

        foreach (ClimbData climb in PersistentInfo.Climbs)
        {
            AddToScroll(climb);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);
    }

    void AddToScroll(ClimbData climb)
    {
        GameObject item = Instantiate(dataItem, scrollContent.transform);

        item.transform.Find("Title").GetComponent<Text>().text = climb.Title;
        item.transform.Find("Details").GetComponent<Text>().text = climb.Info;
        item.GetComponent<Button>().onClick.AddListener(delegate { SelectClimb(climb); });
        GraphDrawer.Draw(item.transform.Find("GraphContainer").gameObject, climb.accelerometer);
    }

    void SelectClimb(ClimbData climb)
    {
        PersistentInfo.CurrentClimb = climb;
        SceneManager.LoadScene("ViewClimb");
    }
}
