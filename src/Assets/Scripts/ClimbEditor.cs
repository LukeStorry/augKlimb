﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ClimbEditor : MonoBehaviour
{
    private ClimbData climb;
    public GameObject confirmationBoxPrefab;

    void Start()
    {
        climb = PersistentInfo.CurrentClimb;

        gameObject.transform.Find("Title").GetComponent<InputField>().onEndEdit.AddListener(input =>
            {
                gameObject.transform.Find("Info - clicktoedit").gameObject.SetActive(false);
                climb.Title = input;
            });

        gameObject.transform.Find("Share Button").GetComponent<Button>().onClick.AddListener(Share);
        gameObject.transform.Find("Crop Button").GetComponent<Button>().onClick.AddListener(Crop);
        gameObject.transform.Find("Bin Button").GetComponent<Button>().onClick.AddListener(Delete);

    }

    void Share()
    {
        string platform = Application.platform.ToString();
        string filepath = FileHandler.ClimbPath(climb);

        if (platform.Contains("Windows")) Application.OpenURL(filepath);
        else new NativeShare().AddFile(filepath).Share();
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