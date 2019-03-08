using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour {

	// Moves to the 
	public void GoToScene (string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    // Moves to the video scene
    public void OpenLink (string url) {
        Application.OpenURL(url);
    }
}
