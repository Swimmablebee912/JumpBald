using System;
using System.Collections.Generic;
using UnityEngine;

public class Visual_FollowTarget : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool inUpdate;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform forNull;
    [SerializeField][Range(0,1)] private float offsetForce = 2f;
    [SerializeField]private float distanceZ = 0f;

    [Header("Read")]
    [SerializeField] private List<Transform> targets;
    [SerializeReference] private Rect rectLimits = new Rect();
    [SerializeReference] private Vector3 offset;
    
    

    private void Start() { SetLimits(); }
    private void Update()  { if (!inUpdate) return; Follow();  }

    public void ResetTarget () { targets.Clear(); targets = new List<Transform>(); }
    private void Follow()  {
        ComprobationNull();

        Vector3 newPosition = Vector3.zero;
        if(targets!=null) foreach (Transform target in targets) { if(target!=null) newPosition += target.position; }
        
        newPosition = newPosition / targets.Count;
        newPosition.z = distanceZ;
        newPosition += offset;
        
        if(newPosition.x>rectLimits.xMax) { offset.x += Math.Clamp(newPosition.x, rectLimits.xMin, rectLimits.xMax) - newPosition.x; }
        else if(newPosition.x<rectLimits.xMin) { offset.x += Math.Clamp(newPosition.x, rectLimits.xMin, rectLimits.xMax) - newPosition.x; }
        if(newPosition.y>rectLimits.yMax) { offset.y += Math.Clamp(newPosition.y, rectLimits.yMin, rectLimits.yMax) - newPosition.y; }
        else if(newPosition.y<rectLimits.yMin) { offset.y += Math.Clamp(newPosition.y, rectLimits.yMin, rectLimits.yMax) - newPosition.y; }

        newPosition.x = Mathf.Clamp(newPosition.x, rectLimits.xMin, rectLimits.xMax);
        newPosition.y = Mathf.Clamp(newPosition.y, rectLimits.yMin, rectLimits.yMax);
        
        pivot.position = newPosition;
    }

    private void ComprobationNull() {
        if (targets.Count == 0) AddTarget(forNull);
        else if (targets.Count > 1) { foreach (Transform target in targets) { if (target == forNull) { targets.Remove(target); return; } } }
    }

    public void AddTarget(Transform target) { foreach (Transform targ in targets) { if (targ == target) return; } targets.Add(target); }
    public void RemoveTarget(Transform target) {
        foreach (Transform targ in targets)  {
            if (targ == target) {
                forNull.position=targ.position;
                targets.Remove(target);
                return;
            }
        }
    }
    
    public void SetOffset(Vector3 newOffset) { offset=newOffset; }
    public void AddOffset(Vector3 direction) {
        Vector3 newDistance = (offset + direction * offsetForce * 10) + pivot.position;
        offset = newDistance - pivot.position;
    }
    public void ResetOffset() { offset=Vector3.zero; }

    public void SetLimits(){
        LimitsCamera limits = CameraManager.main.GetLimits();
        rectLimits.min = limits.min;
        rectLimits.max = limits.max;
    }
    
    private void OnDrawGizmos() {
        if(rectLimits!=null){
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(rectLimits.center, rectLimits.size);
        }
    }
}
