using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ButtonHandler : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public void OpenFolder(string folder)
    {
        Application.OpenURL(Path.Combine(Application.persistentDataPath, folder));
    }
}
