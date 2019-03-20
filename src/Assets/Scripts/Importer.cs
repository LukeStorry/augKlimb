﻿using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace GracesGames.SimpleFileBrowser.Scripts
{

    public class Importer : MonoBehaviour
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
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(ViewMode.Portrait);
            fileBrowserScript.OpenFilePanel(new string[] { "txt", "mp4", "mov" });
            fileBrowserScript.OnFileSelect += TryImportFile;  // subscribes to event (calls using path) 
        }

        // selects and parses an external climb file, then saves to data folder (cached). Displays exceptions in the textbox of attached object.
        private void TryImportFile(string path)
        {
            try
            {
                if (Path.GetExtension(path) == ".txt")
                {
                    ClimbData climb = FileHandler.LoadClimb(path, tryMatch: true);
                    FileHandler.SaveClimb(climb);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else
                {
                    gameObject.GetComponent<Text>().text =  FileHandler.ImportVideo(path);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                gameObject.GetComponent<Text>().text =  e.Message;
            }
            StartCoroutine(ResetText());
        }

        private IEnumerator ResetText()
        {
            yield return new WaitForSeconds(5);
            gameObject.GetComponent<Text>().text = originalText;
        }
    }
}