using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.IO;

public class ClimbViewer : MonoBehaviour
{
    private GameObject graphContainer, scrollView;
    private RectTransform marker;
    private ClimbData climb;
    private VideoPlayer vidPlayer;
    private ScrollRect scrollBar;
    private float graphWidth;
    private int vidFramesOffset;
    private bool videoFrameSelectorMode = false;

    void Start()
    {
        climb = PersistentInfo.CurrentClimb;

        gameObject.transform.Find("Title").GetComponent<InputField>().text = climb.Title;
        gameObject.transform.Find("Title").GetComponent<InputField>().onEndEdit.AddListener(input =>
        {
            gameObject.transform.Find("Info - clicktoedit").gameObject.SetActive(false);
            climb.Title = input;
        });
        gameObject.transform.Find("Details").GetComponent<Text>().text = climb.Details;
        gameObject.transform.Find("Share Button").GetComponent<Button>().onClick.AddListener(Share);
        gameObject.transform.Find("Crop Button").GetComponent<Button>().onClick.AddListener(Crop);
        gameObject.transform.Find("Bin Button").GetComponent<Button>().onClick.AddListener(Delete);
        gameObject.transform.Find("Video Button").GetComponent<Button>().onClick.AddListener(delegate { videoFrameSelectorMode = false; });

        scrollView = gameObject.transform.Find("Scroll View").gameObject;
        scrollView.GetComponent<Button>().onClick.AddListener(delegate { videoFrameSelectorMode = true; });
        scrollBar = scrollView.GetComponent<ScrollRect>();
        Transform scrollContent = scrollView.transform.Find("Viewport").transform.Find("Content");
        graphContainer = scrollContent.Find("GraphContainer").gameObject;
        marker = scrollContent.Find("Marker").GetComponent<RectTransform>();

        float graphHeight = graphContainer.GetComponent<RectTransform>().rect.height;
        if (File.Exists(climb.video))
        {
            Debug.Log("Preparing video: " + climb.video);
            graphHeight *= 0.4f;
            vidPlayer = gameObject.transform.Find("Video").GetComponent<VideoPlayer>();
            vidPlayer.url = climb.video; //"https://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";
            vidFramesOffset = (int)(climb.videoOffset * vidPlayer.frameRate);
        }

        graphWidth = 100f * climb.TimeTaken;
        graphContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(graphWidth, graphHeight);
        scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + graphWidth, graphHeight);
        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(-30, graphHeight + 40);

        GraphDrawer.Draw(graphContainer, climb.accelerometer, includeDots: true, includeSeconds: true);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(PersistentInfo.previousScene);

        if (vidPlayer != null)
        {
            //    if (Input.touchCount > 0)
            //    {
            //        Touch touch = Input.GetTouch(0);
            //        Debug.Log(touch.phase);
            //        if (touch.phase != TouchPhase.Ended)
            //        {
            //            long selectedFrame = vidFramesOffset + (long)(vidPlayer.frameRate * climb.TimeTaken * scrollBar.horizontalNormalizedPosition);
            //            selectedFrame = (long)Mathf.Clamp(selectedFrame, 1, vidPlayer.frameCount);
            //            if (Mathf.Abs(vidPlayer.frame - selectedFrame) > 1)
            //                vidPlayer.frame = selectedFrame;
            //        }
            //    }
            //    else
            //    {
            //        // no touches, scroll the bar to the video
            //        float scrollPos = (vidPlayer.frame - vidFramesOffset) / (vidPlayer.frameRate * climb.TimeTaken);
            //        scrollBar.horizontalNormalizedPosition = Mathf.Clamp(scrollPos, 0f, 1f);
            //    }

            if (videoFrameSelectorMode)
            {
                long selectedFrame = vidFramesOffset + (long)(vidPlayer.frameRate * climb.TimeTaken * scrollBar.horizontalNormalizedPosition);
                selectedFrame = (long)Mathf.Clamp(selectedFrame, 1, vidPlayer.frameCount);
                if (Mathf.Abs(vidPlayer.frame - selectedFrame) > 1)
                    vidPlayer.frame = selectedFrame;
            }
            else
            {
                float scrollPos = (vidPlayer.frame - vidFramesOffset) / (vidPlayer.frameRate * climb.TimeTaken);
                scrollBar.horizontalNormalizedPosition = Mathf.Clamp(scrollPos, 0f, 1f);
            }
        }


        marker.localPosition = new Vector2(graphWidth * scrollBar.horizontalNormalizedPosition + 10, 0);

    }

    void EditTitle()
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

    void Delete()
    {
        GameObject confirmationDialog = Instantiate(Resources.Load("Confimation Box") as GameObject, gameObject.transform);
        confirmationDialog.transform.Find("Message").gameObject.GetComponent<Text>().text = "Are you sure you want to delete this climb?";
        confirmationDialog.transform.Find("No Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate { Destroy(confirmationDialog); });
        confirmationDialog.transform.Find("Yes Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            FileHandler.RemoveClimb(climb);
            SceneManager.LoadScene(PersistentInfo.previousScene);
        });
    }

}


