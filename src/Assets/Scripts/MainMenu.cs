using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        gameObject.transform.Find("Accelero Button").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("AccRecord"); });
        //gameObject.transform.Find("Video Button").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("Video"); });
        gameObject.transform.Find("ViewData Button").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("ViewAllData"); });
        //gameObject.transform.Find("???").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("???"); });

        gameObject.transform.Find("Update Button").GetComponent<Button>().onClick.AddListener(delegate { Application.OpenURL("http://bit.ly/akapk"); });
        gameObject.transform.Find("About Button").GetComponent<Button>().onClick.AddListener(delegate { Application.OpenURL("https://lukestorry.co.uk/augKlimb/"); });
        gameObject.transform.Find("Feedback Button").GetComponent<Button>().onClick.AddListener(delegate { Application.OpenURL("https://goo.gl/forms/GGdaJq2hBdIhsOKX2"); });

        gameObject.transform.Find("Version Text").GetComponent<Text>().text += Application.version;
    }

}
