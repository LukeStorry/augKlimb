using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleFileBrowser;

public class Importer : MonoBehaviour
{
    public Button importButton;
    public bool text;
    public bool video;
    public GameObject fileBrowserPrefab, confirmationBoxPrefab;

    void Start()
    {
        if (!(text ^ video)) throw new Exception("Either text OR video importer must be selected");

        if (text)
            importButton.onClick.AddListener(LaunchFileBrowser);

        if (video)
            importButton.onClick.AddListener(LaunchGalleryBrowser);
    }

    // launches a platform-specific Gallery browser and returns the path of the selected file
    private void LaunchGalleryBrowser()
    {
        NativeGallery.GetVideoFromGallery(TryImportFile, "Pick a Video", "video/*");
    }

    // launches a platform-specific file browser and returns the path of the selected file
    private void LaunchFileBrowser()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Climb Files", ".txt"));
        FileBrowser.SetDefaultFilter(".txt");
        FileBrowser.ShowLoadDialog(TryImportFile, null, false, "Load Climb File", "Select a Climb File");

    }

    // selects and parses an external file, then saves to data folder (cached). Displays exceptions in the textbox of attached object.
    private void TryImportFile(string path)
    {
        try
        {
            if (video)
                FileHandler.ImportVideo(path);
            else if (text)
                FileHandler.LoadClimb(path, copy: true);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            DisplayText(e.Message);
        }
    }


    private void DisplayText(string text)
    {
        GameObject confirmationDialog = Instantiate(confirmationBoxPrefab, gameObject.transform);
        confirmationDialog.transform.Find("Message").gameObject.GetComponent<Text>().text = text;
        confirmationDialog.transform.Find("No Button").gameObject.GetComponent<Text>().text = "ok";
        confirmationDialog.transform.Find("No Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(confirmationDialog);
        });
        confirmationDialog.transform.Find("Yes Button").gameObject.SetActive(false);
    }


}