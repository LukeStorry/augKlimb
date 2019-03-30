using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;


public class ClimbEditor : MonoBehaviour
{
    public GameObject confirmationBoxPrefab;

    private ClimbData climb;
    private InputField offsetField;

    void Start()
    {
        climb = PersistentInfo.CurrentClimb;

        gameObject.transform.Find("Title").GetComponent<InputField>().onEndEdit.AddListener(input =>
            {
                climb.Title = input;
                gameObject.transform.Find("Info - clicktoedit").gameObject.SetActive(false);
            });

        gameObject.transform.Find("Crop Button").GetComponent<Button>().onClick.AddListener(Crop);
        gameObject.transform.Find("Bin Button").GetComponent<Button>().onClick.AddListener(Delete);


        GameObject offsetEditorBox = gameObject.transform.Find("Video Offset Editor").gameObject;
        if (File.Exists(climb.VideoPath))
        {

            offsetField = offsetEditorBox.transform.Find("Number").GetComponent<InputField>();
            offsetField.text = climb.VideoOffset.ToString("0.0");
            offsetField.onEndEdit.AddListener(input => climb.VideoOffset = float.Parse(input));

            offsetEditorBox.transform.Find("Plus Button").GetComponent<Button>().onClick.AddListener(delegate
            {
                climb.VideoOffset += 0.1f;
                offsetField.text = climb.VideoOffset.ToString("0.0");
            });
            offsetEditorBox.transform.Find("Minus Button").GetComponent<Button>().onClick.AddListener(delegate
            {
                climb.VideoOffset -= 0.1f;
                offsetField.text = climb.VideoOffset.ToString("0.0");
            });
        }
        else
        {
            offsetEditorBox.SetActive(false);
        }

    }

    void Crop()
    {
        float scrollPosition = gameObject.transform.Find("Scroll View").GetComponent<ScrollRect>().horizontalNormalizedPosition;

        GameObject confirmationDialog = Instantiate(confirmationBoxPrefab, gameObject.transform);
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
        GameObject confirmationDialog = Instantiate(confirmationBoxPrefab, gameObject.transform);
        confirmationDialog.transform.Find("Message").gameObject.GetComponent<Text>().text = "Are you sure you want to delete this climb?";
        confirmationDialog.transform.Find("No Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate { Destroy(confirmationDialog); });
        confirmationDialog.transform.Find("Yes Button").gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            FileHandler.RemoveClimb(climb);
            SceneManager.LoadScene(PersistentInfo.previousScene);
        });
    }

}
