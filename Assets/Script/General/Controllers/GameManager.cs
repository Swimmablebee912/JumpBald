using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;


public class GameManager : MonoBehaviour
{
    public static GameManager main;
    private void Awake() { if(main==null) main=this; else Destroy(gameObject);  Application.targetFrameRate=60; }

    private void Start() { system_GameFlow.InitialScene(); }

    public SystemGameFlow system_GameFlow;
    public SystemSettings system_Settings;

    [System.Serializable] public class SystemGameFlow { 
        [Header("Configuration")]
        public string firstLoadedScreen = "Intro";

        public void InitialScene() { SceneLoaderManager.main.LoadScene(ScreenFlow.main.GetScreenData(firstLoadedScreen).sceneLoad); }
        public void ChangeScene(string nameScreen) { ScreenFlow.main.ChangeScreen(nameScreen); }
        public void ChangeSubScene(string subScene) { ScreenFlow.main.ChangeScreen( ScreenFlow.main.currentScreen.data.nameScene, subScene); }
        public void ChangeSceneAndSubScene(string nameScreen, string subScene) { ScreenFlow.main.ChangeScreen( nameScreen, subScene); }
        public void ChangeSceneReload(string nameScreen) { ScreenFlow.main.ChangeScreen(nameScreen, true); }
        public void ChangeSceneAndSubSceneReload(string nameScreen, string subScene) { ScreenFlow.main.ChangeScreen( nameScreen, subScene, true); }

    }
    [System.Serializable] public class SystemSettings { 
        public void ChangeSetting() {  }
    }
    
}
