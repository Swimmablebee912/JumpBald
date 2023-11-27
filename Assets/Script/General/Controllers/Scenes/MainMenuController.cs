using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController main; private void Awake()  { if(main==null) { main=this;} else { Destroy(gameObject); }}

    [Header("Reference")]
    public List<UIElement_LoadRun> uIElement_LoadRun;

    public void Function_Button_toNewRun(string nameRun) { SaveLoadController.main.SetData_CurrentSave(new RunData{ name = nameRun }); GameManager.main.system_GameFlow.ChangeScene("Gameplay"); }
    public void Function_Button_toLoadRun(RunData toLoad) { SaveLoadController.main.SetData_CurrentSave(toLoad); GameManager.main.system_GameFlow.ChangeScene("Gameplay"); }

    public void Function_Button_toMenu() { GameManager.main.system_GameFlow.ChangeSubScene("MainMenu"); }
    public void Function_Button_toStartGame () { GameManager.main.system_GameFlow.ChangeScene("Gameplay"); }
    public void Function_Button_toLoadGame () { LoadData(); GameManager.main.system_GameFlow.ChangeSubScene("Load"); }
    public void Function_Button_toOptions () {  GameManager.main.system_GameFlow.ChangeSubScene("Options"); }
    public void Function_Button_toCredits () {  GameManager.main.system_GameFlow.ChangeSubScene("Credits"); }

    public void LoadData() {
        List<RunData> dataSave = SaveLoadController.main.GetRuns();
        if(dataSave!=null) {
            int idSave = 0;
            foreach(UIElement_LoadRun uiElement in uIElement_LoadRun) { 
                if(idSave >= dataSave.Count) {  uiElement.SetData(null, !(idSave > dataSave.Count));  } 
                else { uiElement.SetData(dataSave[idSave], true);  }
                idSave++; 
            }
        }
    }
}
