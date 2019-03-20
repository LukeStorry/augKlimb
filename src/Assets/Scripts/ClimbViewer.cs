using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ClimbViewer : MonoBehaviour
{
    private GameObject graphContainer, scrollView;
    private RectTransform marker;
    private ClimbData climb;
    private VideoPlayer vidPlayer;
    private ScrollRect scrollBar;
    private float graphWidth;
    private int vidFramesOffset;

    void Start()
    {
        climb = PersistentInfo.CurrentClimb;
        gameObject.transform.Find("Title").GetComponent<Text>().text = climb.Date.ToString("F", null);
        gameObject.transform.Find("Details").GetComponent<Text>().text = climb.InfoText.Replace("\n", ", ");
        gameObject.transform.Find("Share Button").GetComponent<Button>().onClick.AddListener(Share);
        gameObject.transform.Find("Crop Button").GetComponent<Button>().onClick.AddListener(Crop);
        gameObject.transform.Find("Bin Button").GetComponent<Button>().onClick.AddListener(Delete);

        scrollView = gameObject.transform.Find("Scroll View").gameObject;
        scrollBar = scrollView.GetComponent<ScrollRect>();
        Transform scrollContent = scrollView.transform.Find("Viewport").transform.Find("Content");
        graphContainer = scrollContent.Find("GraphContainer").gameObject;
        marker = scrollContent.Find("Marker").GetComponent<RectTransform>();

        float graphHeight = graphContainer.GetComponent<RectTransform>().rect.height;
        if (climb.video != "") 
        {
            graphHeight *= 0.4f;
            vidPlayer = gameObject.transform.Find("Video").GetComponent<VideoPlayer>();
            vidPlayer.url = climb.video;
            vidFramesOffset = (int) (climb.videoOffset * vidPlayer.frameRate);
            Debug.Log("Calculated offset frames: " + vidFramesOffset + " for vid: " + climb.video);
            InvokeRepeating("VideoScroller", 0, 0.1f);
        }

        graphWidth = 100f * climb.TimeTaken;
        graphContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(graphWidth, graphHeight);
        scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + graphWidth, graphHeight);
        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(-30, graphHeight + 40);

        GraphDrawer.Draw(graphContainer, climb.accelerometer, includeDots: true, includeSeconds: true);

    }

    private void Update()
    {
        marker.localPosition = new Vector2(graphWidth * scrollBar.horizontalNormalizedPosition +10, 0);
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("ViewAllData");
    }

    private void VideoScroller() { 
        // TODO calculate time difference of start points, as offset
        vidPlayer.frame = vidFramesOffset + (long)(vidPlayer.frameRate * climb.TimeTaken * scrollBar.horizontalNormalizedPosition);
        Debug.Log("FrameRate: " + vidPlayer.frameRate);
        Debug.Log("Showing Frame: " + vidPlayer.frame + "/" + vidPlayer.frameCount);
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

        GameObject confirmationDialog = Instantiate(Resources.Load("Confimation Box") as GameObject, gameObject.transform);
        string message = System.String.Format("Are you sure you want to delete all climb data after {0:0.0} seconds?", scrollPosition * climb.TimeTaken);
        confirmationDialog.transform.Find("Message").gameObject.GetComponent<Text>().text = message;
        confirmationDialog.transform.Find("No Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(confirmationDialog);
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


