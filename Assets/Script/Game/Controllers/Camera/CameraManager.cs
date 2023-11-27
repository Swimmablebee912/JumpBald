using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager main;
    private void Awake() { if(main!=null) Destroy(this.gameObject); else main=this; }

    [SerializeField] Visual_FollowTarget scr_Follow;
    [SerializeField] Visual_CameraShakeCM scr_CameraShake;
    [SerializeField] CameraPespectiveConfiner2D scr_Confiner;
    [SerializeField] ScreenShakes[] shakes;

    public void ResetTarget() { scr_Follow.ResetTarget(); }
    public void AddTarget(Transform target) { scr_Follow.AddTarget(target); }
    public void RemoveTarget(Transform target) {  scr_Follow.RemoveTarget(target);}
    public void SetOffset(Vector3 offset) { scr_Follow.SetOffset(offset); }
    public void AddOffset(Vector3 offset) { scr_Follow.AddOffset(offset); }
    public void ResetOffset() { scr_Follow.ResetOffset(); }

    public LimitsCamera GetLimits(){ return scr_Confiner.GetLimit();}

    public void AddShake(string nameShake) {   foreach(ScreenShakes shake in shakes){ if(shake.name==nameShake) {shake.Activate(); return;} } }

    public void SetCameraConfinder(Bounds bounds) {  scr_Confiner.ChangeSize(bounds); scr_Follow.SetLimits(); }
    //public void SetCameraConfinder(){ scr_Confiner.SetConfinders();}

}