using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.IO;
using System.Collections;

public class ClimbViewer : MonoBehaviour
{
    private GameObject graphContainer, scrollView;
    private RectTransform marker;
    private ClimbData climb;
    private VideoPlayer vidPlayer;
    private ScrollRect scrollBar;
    private float graphWidth;
    private bool videoFrameSelectorMode = true;

    void Start()
    {
        climb = PersistentInfo.CurrentClimb;

        gameObject.transform.Find("Title").GetComponent<InputField>().text = climb.Title;
        gameObject.transform.Find("Details").GetComponent<Text>().text = climb.Details;
        gameObject.transform.Find("Share Button").GetComponent<Button>().onClick.AddListener(Share);
        gameObject.transform.Find("Video Overlay Button").GetComponent<Button>().onClick.AddListener(delegate { videoFrameSelectorMode = false; });

        scrollView = gameObject.transform.Find("Scroll View").gameObject;
        scrollView.GetComponent<Button>().onClick.AddListener(delegate { videoFrameSelectorMode = true; });

        scrollBar = scrollView.GetComponent<ScrollRect>();
        Transform scrollContent = scrollView.transform.Find("Viewport").transform.Find("Content");
        graphContainer = scrollContent.Find("GraphContainer").gameObject;
        marker = scrollContent.Find("Marker").GetComponent<RectTransform>();

        float graphHeight = graphContainer.GetComponent<RectTransform>().rect.height;
        if (File.Exists(climb.VideoPath))
        {
            graphHeight *= 0.4f;
            vidPlayer = gameObject.transform.Find("Video").GetComponent<VideoPlayer>();
            vidPlayer.url = climb.VideoPath;
        }
        else gameObject.transform.Find("Video").gameObject.SetActive(false);

        graphWidth = 100f * climb.TimeTaken;
        graphContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(graphWidth, graphHeight);
        scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + graphWidth, graphHeight);
        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(-10, graphHeight + 20);

        new GraphDrawer(graphContainer, climb.accelerometer).Draw(includeDots: true, includeSeconds: true);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(PersistentInfo.previousScene);

        if (vidPlayer != null)
        {
            int vidFramesOffset = (int)(climb.VideoOffset * vidPlayer.frameRate);

            if (videoFrameSelectorMode)
            {
                long selectedFrame = vidFramesOffset + (long)(vidPlayer.frameRate * climb.TimeTaken * scrollBar.horizontalNormalizedPosition);
                selectedFrame = (long)Mathf.Clamp(selectedFrame, 1, vidPlayer.frameCount);
                if (Mathf.Abs(vidPlayer.frame - selectedFrame) > 2)
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


    void Share()
    {
        string platform = Application.platform.ToString();
        string filepath = FileHandler.ClimbPath(climb);

        if (platform.Contains("Windows")) Application.OpenURL(filepath);
        else new NativeShare().AddFile(filepath).Share();
    }
}


