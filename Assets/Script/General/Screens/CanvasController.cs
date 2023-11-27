using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public CanvasData[] canvasData;

    void Start(){ SetData(); }
    
    void SetData(){  foreach(CanvasData canvaData in canvasData){  canvaData.Set(); } }
    public void SetSubScreen(string subScreen) { foreach(CanvasData canvaData in canvasData){ if(canvaData.isCanvasMenu) {if(canvaData.SetSubScreen(subScreen)) return;} }  }

    public Animator GetAnimator(){ foreach(CanvasData canvaData in canvasData){ if(canvaData.isCanvasMenu) return canvaData.anim; } return null; }
}

[System.Serializable]
public class CanvasData {
    public string nameCanvas;
    public bool isCanvasMenu = true;
    public Canvas canvas;
    public Animator anim;

    public ScreenData data;
    

    public void Set() {  canvas.worldCamera = Camera.main; } 

    public bool SetSubScreen (string subScreen) { 
         foreach(SubScenesData subScreenData in data.subScenes){ 
            if (subScreenData.name == subScreen) {
                if(anim!=null) anim.SetTrigger(subScreenData.nameTriggerToAnimator);
                return true;
            }
        }
        return false;
    }

}