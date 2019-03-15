using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClimbImporter : MonoBehaviour
{
    private string originalText;

    void Start()
    {
        originalText = gameObject.GetComponent<Text>().text;
        gameObject.GetComponent<Button>().onClick.AddListener(ImportFile);
    }

    private void ImportFile()
    {
        string path = selectFile();
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

    private string selectFile()
    {
        string path = "";
        string platform = Application.platform.ToString();

#if UNITY_EDITOR
            path = EditorUtility.OpenFilePanel("Select Climb File", "", "txt");
            Debug.Log(path);

#elif UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
            path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<String>("getAbsolutePath");
        }
        catch (e)
        {
            Debug.Log(e.Message);
        }
#endif

        // TODO add support for other platforms (mainly IOS)
        return path;
    }
}

