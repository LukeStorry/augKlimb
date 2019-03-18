using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ClimbViewer : MonoBehaviour
{
    public GameObject backButton, title, details, shareButton, cropButton, binButton, video, scrollView, scrollContent, scrollBar;

    private GameObject graphContainer;
    private ClimbData climb;
    private VideoPlayer vidPlayer;

    void Start()
    {
        climb = PersistentInfo.CurrentClimb;

        backButton.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("ViewAllData"); });
        title.GetComponent<Text>().text = climb.Date.ToString("F", null);
        details.GetComponent<Text>().text = climb.InfoText.Replace("\n", ", ");
        shareButton.GetComponent<Button>().onClick.AddListener(Share);
        cropButton.GetComponent<Button>().onClick.AddListener(Crop);
        binButton.GetComponent<Button>().onClick.AddListener(Delete);

        graphContainer = scrollContent.transform.Find("GraphContainer").gameObject;

        float graphHeight = graphContainer.GetComponent<RectTransform>().rect.height;
        if (climb.video != null) 
        {
            graphHeight *= 0.4f;
            vidPlayer = video.GetComponent<VideoPlayer>();
            vidPlayer.url = climb.video;
            InvokeRepeating("VideoScroller", 0, 0.1f);
        }

        float graphWidth = 100f * climb.TimeTaken;
        graphContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(graphWidth, graphHeight);
        scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + graphWidth, graphHeight);
        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(-30, graphHeight + 40);

        GraphDrawer.Draw(graphContainer, climb.accelerometer, includeDots: true, includeSeconds: true);

    }

    private void VideoScroller() { 
        // TODO calculate time difference of start points, as offset
        vidPlayer.frame = (long)(vidPlayer.frameCount * scrollView.GetComponent<ScrollRect>().horizontalNormalizedPosition);
        float scrollPosition = scrollView.GetComponent<ScrollRect>().horizontalNormalizedPosition;
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

    void Share()
    {
        string platform = Application.platform.ToString();
        string filepath = FileHandler.ClimbPath(climb);

        if (platform.Contains("Windows")) Application.OpenURL(filepath);
        else new NativeShare().AddFile(filepath).Share(); 
    }
}


