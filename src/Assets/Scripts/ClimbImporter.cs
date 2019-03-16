using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace GracesGames.SimpleFileBrowser.Scripts
{

    public class ClimbImporter : MonoBehaviour
    {
        public GameObject FileBrowserPrefab;
        private string originalText;

        void Start()
        {
            originalText = gameObject.GetComponent<Text>().text;
            gameObject.GetComponent<Button>().onClick.AddListener(LaunchFileBrowser);
        }

        // launches a platform-specific file browser and returns the path of the selected file
        private void LaunchFileBrowser()
        {
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            //fileBrowserObject.name = "FileBrowser";
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(ViewMode.Portrait);
            fileBrowserScript.OpenFilePanel(new string[] { "txt" });
            fileBrowserScript.OnFileSelect += ImportFile;  // subscribes to event (calls using path) 
        }

        // selects and parses an external climb file, then saves to data folder (cached). Displays exceptions in the textbox of attached object.
        private void ImportFile(string path)
        {
            try
            {
                ClimbData climb = FileHandler.LoadClimb(path);
                FileHandler.SaveClimb(climb);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                gameObject.GetComponent<Text>().text = e.Message;
                StartCoroutine(ResetText());
            }

        }

        IEnumerator ResetText()
        {
            yield return new WaitForSeconds(3);
            gameObject.GetComponent<Text>().text = originalText;
        }
    }

}