using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroController : MonoBehaviour
{
    [Header("Reference")]
    public TextMeshProUGUI text;
    public Animator anim;

    [Header("Configuration")]
    public float secondToClose = 2f;
    public string nextScreen = "MainMenu";
    public bool withLoader = false;

    private void Start()  { 
        AudioManager.main.PlayMUSIC("Stop");
        text.SetText("Version " + Application.version);
        StartCoroutine(NextScene()); 
    }

    private IEnumerator NextScene () {
        yield return new WaitForSeconds(secondToClose);
        anim.SetTrigger("Close");
        yield return new WaitForSeconds(1);
        GameManager.main.system_GameFlow.ChangeScene(nextScreen);
    }
}
