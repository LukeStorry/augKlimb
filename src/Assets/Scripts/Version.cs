using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour {
    void Start()
    {
        GetComponent<Text>().text += Application.version;
    }
}
