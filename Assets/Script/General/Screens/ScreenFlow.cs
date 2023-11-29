using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFlow : MonoBehaviour
{
    public static ScreenFlow main;
    private void Awake() { if(main!=null) Destroy(this.gameObject); else main=this; }

    public Screen_InGame currentScreen;
    public Screen_InGame[] Screens;

    void Start () { Initialize(); }

    void Initialize(){
         foreach(Screen_InGame screen_ in Screens){
            if(screen_.data.sceneLoad==SceneManager.GetActiveScene().name) { currentScreen = screen_;  return;}
        }
    }

    public void ChangeScreen(string toScreen){
        foreach(Screen_InGame screen_ in Screens){
            if(screen_.data.nameScene==toScreen){
                if (currentScreen.data.nameScene != screen_.data.nameScene) currentScreen.Close();
                screen_.Load(false);
                currentScreen = screen_;
                return;
            }
        }
    }
    public void ChangeScreen(string toScreen, string toSubScreen){
        foreach(Screen_InGame screen_ in Screens){
            if(screen_.data.nameScene==toScreen){
                if (currentScreen.data.nameScene != screen_.data.nameScene) currentScreen.Close();
                screen_.Load(toSubScreen, false);
                currentScreen = screen_;
                return;
            }
        }
    }
    public void ChangeScreen(string toScreen, bool Reload){
        foreach(Screen_InGame screen_ in Screens){
            if(screen_.data.nameScene==toScreen){
                if (currentScreen.data.nameScene != screen_.data.nameScene || Reload) currentScreen.Close();
                screen_.Load(Reload);
                currentScreen = screen_;
                return;
            }
        }
    }
    public void ChangeScreen(string toScreen, string toSubScreen, bool Reload){
        foreach(Screen_InGame screen_ in Screens){
            if(screen_.data.nameScene==toScreen){
                if (currentScreen.data.nameScene != screen_.data.nameScene || Reload) currentScreen.Close();
                screen_.Load(toSubScreen, Reload);
                currentScreen = screen_;
                return;
            }
        }
    }
    public ScreenData GetScreenData(string toScreen){
        foreach(Screen_InGame screen_ in Screens){ if(screen_.data.nameScene==toScreen) return screen_.data; }
        return null;
    }
}

[System.Serializable]
public class Screen_InGame {
    public ScreenData data;
    
    public void Load(bool Reload){ 
        SceneLoaderManager.main.LoadScene_Asyncronic(data.sceneLoad, Reload); 
    }
    public void Load(string subScene, bool Reload){ Load(Reload);
        if(data.objCanvas != null) {
            GameObject objectFinded = GameObject.Find(data.objCanvas.name);
            if(objectFinded!=null) { objectFinded.GetComponent<CanvasController>().SetSubScreen(subScene); }
        }
        foreach(SubScenesData subSceneData in data.subScenes){
            if(subSceneData.name==subScene) {
                if(subSceneData.canStopTime) { Time.timeScale=0; } else { Time.timeScale=1; }
                break;
            } 
        }
    }
    public void Close(){ 
        if(data.objCanvas != null) {
            GameObject objectFinded = GameObject.Find(data.objCanvas.name);
            if(objectFinded!=null) { objectFinded.GetComponent<CanvasController>().SetSubScreen("Close"); }
        }
    }
}