using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIElement_LoadRun : MonoBehaviour
{
    [Header("Reference")]
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textLife;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textTitle;
    public TMP_InputField textInput;
    public Button button;
    
    [Header("Read")]
    public bool loadedRun;
    private RunData dataSave = null;

    public void SetData(RunData newSave, bool toUse) { 
        dataSave = newSave; 
        loadedRun = dataSave!=null; 

        if(loadedRun) { 
            button.interactable = true; 

            textLevel.gameObject.SetActive(true);
            textLife.gameObject.SetActive(true);
            textTimer.gameObject.SetActive(true);
            textTitle.gameObject.SetActive(false);
            textInput.gameObject.SetActive(false);

            textLevel.SetText( GetLevel() );
            textLife.SetText( GetLife() );
            textTimer.SetText( GetTimer() );

        } else {
            textLevel.gameObject.SetActive(false);
            textLife.gameObject.SetActive(false);
            textTimer.gameObject.SetActive(false);

            if(toUse) {
                button.interactable = true; 

                textTitle.gameObject.SetActive(false);
                textInput.gameObject.SetActive(true);

                textInput.placeholder.GetComponent<TextMeshProUGUI>().text =  "New Run";
            } else {
                button.interactable = false; 

                textTitle.gameObject.SetActive(true);
                textInput.gameObject.SetActive(false);

                textTitle.text =  "Empty Run";
            }
        }
    }

    public void PressButton() {
        if(loadedRun) { MainMenuController.main.Function_Button_toLoadRun(dataSave); }
        else {   }
    }
    public void PressButton_ToNewRun() { if(textInput.text!="") MainMenuController.main.Function_Button_toNewRun(textInput.text); }

    public string GetLevel() { return dataSave.currentRoom.data.stage.data.name + " " + dataSave.currentRoom.GetIdRoom(); }
    public string GetLife() { string text = ""; for(int i=1; i<=dataSave.lifeMax ;i++){ text += i<=dataSave.life?"<color=#fff><sprite=2></color>":"<sprite=3>"; } return text; }
    public string GetTimer() { return string.Format("{0}:{1}:{2}", Mathf.FloorToInt(dataSave.time / 60), Mathf.FloorToInt(dataSave.time % 60),  Mathf.FloorToInt((dataSave.time * 1000000) % 100)); }

}