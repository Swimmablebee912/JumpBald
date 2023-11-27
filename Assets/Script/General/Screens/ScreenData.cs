using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScreenFlow/Scenes")]
public class ScreenData : ScriptableObject{
    public string nameScene;
    public string sceneLoad;
    public GameObject objCanvas;
    public SubScenesData[] subScenes;
    
}

[System.Serializable] public class SubScenesData{
    public string name;
    public string nameTriggerToAnimator;
    public bool canStopTime;
}