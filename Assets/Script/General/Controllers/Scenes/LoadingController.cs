using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    public static LoadingController main;
    private void Awake() { if(main!=null) Destroy(this.gameObject); else main=this; }

    [Header("Configuration")]
    public List<IllustrationToShow> illustrations;

    [Header("Reference")]
    public Image image_Illustration;
    public TextMeshProUGUI text_Loading;
    public GameObject objectButton;

    private void Start() { 
        AudioManager.main.PlayMUSIC("Stop");
        ShowIllustration(); 
        text_Loading.SetText("Loading"); 
    }

    private void ShowIllustration() {
        objectButton.SetActive(true);
        foreach(IllustrationToShow illustration in illustrations ) { if (SceneLoaderManager.main.nextScene == illustration.nameScene) { image_Illustration.sprite = illustration.imageToLoad; return; }  }
        image_Illustration.sprite = illustrations[0].imageToLoad;
        objectButton.SetActive(false);
    }
    public void ReadyToLoad(){ text_Loading.SetText("Loaded");  AudioManager.main.PlaySFX("UI_Trigger_Menu_Option_PressNormal"); }
 
    public void Function_Button_Loading(){ AudioManager.main.PlaySFX("UI_Trigger_Menu_Option_PressSpecial"); SceneLoaderManager.main.input_LoadAsyncEnd = true;  }

    [System.Serializable] public class IllustrationToShow{
        public string nameScene;
        public Sprite imageToLoad;
    }
}
