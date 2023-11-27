using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroController : MonoBehaviour
{
    [Header("Reference")]
    public TextMeshProUGUI text;

    [Header("Configuration")]
    public float secondToClose = 2f;
    public string nextScreen = "MainMenu";
    public bool withLoader = false;

    private void Start()  { 
        text.SetText("Version " + Application.version);
        StartCoroutine(NextScene()); 
    }

    private IEnumerator NextScene () {
        yield return new WaitForSeconds(secondToClose);
        GameManager.main.system_GameFlow.ChangeScene(nextScreen);
    }
}
