using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClimbViewer : MonoBehaviour
{
    public GameObject scrollContent;
    GameObject graphContainer;
    private ClimbData climb;
    void Start()
    {
        gameObject.transform.Find("Back Button").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("ViewAllData"); });

        climb = PersistentInfo.currentClimb;
        gameObject.transform.Find("Title").GetComponent<Text>().text = climb.Date.ToString("F", null);
        gameObject.transform.Find("Details").GetComponent<Text>().text = climb.InfoText.Replace("\n", ", ");

        graphContainer = scrollContent.transform.Find("GraphContainer").gameObject;

        Vector2 graphSize = new Vector2(100 * climb.TimeTaken, graphContainer.GetComponent<RectTransform>().rect.height);
        graphContainer.GetComponent<RectTransform>().sizeDelta = graphSize;
        scrollContent.GetComponent<RectTransform>().sizeDelta = graphSize + new Vector2(20, 0);

        GraphDrawer.Draw(graphContainer, climb.accelerometer, includeDots: true, includeSeconds: true);

        gameObject.transform.Find("Crop Button").GetComponent<Button>().onClick.AddListener(Crop);
        gameObject.transform.Find("Bin Button").GetComponent<Button>().onClick.AddListener(Delete);
    }

    void Delete()
    {
        GameObject confirmationDialog = Instantiate(Resources.Load("Confimation Box") as GameObject, gameObject.transform);
        confirmationDialog.transform.Find("Message").gameObject.GetComponent<Text>().text = "Are you sure you want to delete this climb?";
        confirmationDialog.transform.Find("No Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate { Destroy(confirmationDialog); });
        confirmationDialog.transform.Find("Yes Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            FileHandler.RemoveClimb(climb);
            SceneManager.LoadScene("ViewAllData");
        });
    }


    void Crop()
    {
        float scrollPosition = gameObject.transform.Find("Scroll View").GetComponent<ScrollRect>().horizontalNormalizedPosition;
        GameObject line = GraphDrawer.VerticalLine(graphContainer, scrollPosition, Color.red);

        GameObject confirmationDialog = Instantiate(Resources.Load("Confimation Box") as GameObject, gameObject.transform);
        string message = System.String.Format("Are you sure you want to delete all climb data after {0:0.0} seconds?", scrollPosition * climb.TimeTaken);
        confirmationDialog.transform.Find("Message").gameObject.GetComponent<Text>().text = message;
        confirmationDialog.transform.Find("No Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(confirmationDialog);
            Destroy(line);
        });
        confirmationDialog.transform.Find("Yes Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            climb.Crop(scrollPosition);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }
}


