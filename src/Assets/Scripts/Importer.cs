using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace GracesGames.SimpleFileBrowser.Scripts
{

    public class Importer : MonoBehaviour
    {
        public Button importButton;
        public GameObject fileBrowserPrefab, confirmationBoxPrefab;
        public string[] filetypes = new string[] { "txt", "mp4", "mov" };

        void Start()
        {
            importButton.onClick.AddListener(LaunchFileBrowser);
        }

        // launches a platform-specific file browser and returns the path of the selected file
        private void LaunchFileBrowser()
        {
            GameObject fileBrowserObject = Instantiate(fileBrowserPrefab, transform);
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(ViewMode.Portrait);
            fileBrowserScript.OpenFilePanel(filetypes);
            fileBrowserScript.OnFileSelect += TryImportFile;  // subscribes to event (calls using path) 
        }

        // selects and parses an external climb file, then saves to data folder (cached). Displays exceptions in the textbox of attached object.
        private void TryImportFile(string path)
        {
            try
            {
                if (Path.GetExtension(path) == ".txt")
                    FileHandler.LoadClimb(path, copy: true);
                else
                    FileHandler.ImportVideo(path);

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
}