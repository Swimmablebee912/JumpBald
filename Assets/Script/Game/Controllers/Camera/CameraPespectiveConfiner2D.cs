using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))]
public class CameraPespectiveConfiner2D : MonoBehaviour
{ 
    [Header("Reference")]
    [SerializeField] private CinemachineVirtualCamera vCamReference;
    public BoxCollider coll;
    [Header("Configuration")]
    [SerializeField] private float zPositionReference;
    [SerializeField] private Vector2 rectBase;
    [SerializeField] private Vector2 boundBase;
    [SerializeField] private Vector2 offset_toFixesSize;

    [Header("Read")]
    public LimitsCamera limits;
    [SerializeField] private Vector2 boundSize;
    private List<BoxCollider> colliderInScene;

    
    [Header("Test")]
    [SerializeField] private Color gizmosColor = Color.blue; [SerializeField] private Color  gizmosColor2 = Color.blue;
    public Transform transformCharacter;

    private void OnDrawGizmos() {

        Gizmos.color = gizmosColor2;
        Vector3 centro = (Camera.main.transform.position);
        Vector3 tamaño = GetCameraRectangle();

        Gizmos.DrawCube(centro, tamaño);


        Gizmos.color = gizmosColor;

        centro = (GetRect().min + GetRect().max) * 0.5f;
        tamaño = GetRect().size;

        Gizmos.DrawWireCube(centro, tamaño);
    }
    
    public void ChangeSize(Bounds bounds){ ChangeSize(bounds.min, bounds.max); GetRect();  }

    [EasyButtons.Button] public void SetConfinders(){
        colliderInScene = new List<BoxCollider>(); 
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RoomConfinder")) {
            BoxCollider boxColl = obj.GetComponent<BoxCollider>();
            if(boxColl!=null) colliderInScene.Add(boxColl);
        }
        List<BoxCollider> colliderActive = new List<BoxCollider>();
        foreach (BoxCollider coll in colliderInScene){
            if(transformCharacter.position.y>GetRect(coll.bounds).min.y && transformCharacter.position.y<GetRect(coll.bounds).max.y) { 
                if(!colliderActive.Contains(coll)) colliderActive.Add(coll); 
            }
        }
        if(colliderActive.Count>0) {
            Bounds bounds = new Bounds  { min = colliderActive[0].bounds.min, max = colliderActive[0].bounds.max };
            foreach (Collider coll in colliderActive){
                if(bounds.min.y<coll.bounds.min.y) { bounds.min = coll.bounds.min; }
                if(bounds.max.y>coll.bounds.max.y) { bounds.max = coll.bounds.max; }
            }
            ChangeSize(bounds);
        }
    }


    public LimitsCamera GetLimit() {
        limits.min = coll.bounds.min;
        limits.max = coll.bounds.max;
        return limits;
    }
    private Rect GetRect(Bounds checkBounds){
        var bounds = checkBounds;
        var downLeft = bounds.min;
        var downRight = new Vector3(bounds.max.x, bounds.min.y);
        var upRight = new Vector3(bounds.max.x, bounds.max.y);

        var dist = vCamReference.transform.position.z - zPositionReference;
        var height = 2.0f * dist * Mathf.Tan(vCamReference.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad);
        var width = height * vCamReference.m_Lens.Aspect;
        
        var cameraRect =  new Vector2(Mathf.Abs(width), Mathf.Abs(height));

        var rectCameraDownLeft = new Rect(downLeft - new Vector3(cameraRect.x / 2, cameraRect.y / 2), cameraRect);
        var rectCameraDownRight = new Rect(downRight - new Vector3(cameraRect.x / 2, cameraRect.y / 2), cameraRect);

        var rectCameraUpLeft = new Rect(upRight - new Vector3(cameraRect.x / 2, cameraRect.y / 2), cameraRect);

        var minDownX = rectCameraDownLeft.min.x;
        var maxDownX = rectCameraDownRight.max.x;

        var minYDown = rectCameraDownLeft.min.y;
        var maxYUp = rectCameraUpLeft.max.y;

        var sizeW = maxDownX - minDownX;
        var sizeH = maxYUp - minYDown;

        var finalRect = new Rect(minDownX, minYDown, sizeW, sizeH);
        
        return finalRect;
    }
    private Rect GetRect() {
        var bounds = coll.bounds;
        var downLeft = bounds.min;
        var downRight = new Vector3(bounds.max.x, bounds.min.y);
        var upRight = new Vector3(bounds.max.x, bounds.max.y);

        var cameraRect = GetCameraRectangle();

        var rectCameraDownLeft = new Rect(downLeft - new Vector3(cameraRect.x / 2, cameraRect.y / 2), cameraRect);
        var rectCameraDownRight = new Rect(downRight - new Vector3(cameraRect.x / 2, cameraRect.y / 2), cameraRect);

        var rectCameraUpLeft = new Rect(upRight - new Vector3(cameraRect.x / 2, cameraRect.y / 2), cameraRect);

        var minDownX = rectCameraDownLeft.min.x;
        var maxDownX = rectCameraDownRight.max.x;

        var minYDown = rectCameraDownLeft.min.y;
        var maxYUp = rectCameraUpLeft.max.y;

        var sizeW = maxDownX - minDownX;
        var sizeH = maxYUp - minYDown;

        var finalRect = new Rect(minDownX, minYDown, sizeW, sizeH);
        
        return finalRect;
    }
    private Vector2 GetCameraRectangle() {
        var dist = vCamReference.transform.position.z - zPositionReference;
        var height = 2.0f * dist * Mathf.Tan(vCamReference.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad);
        var width = height * vCamReference.m_Lens.Aspect;
        
        return new Vector2(Mathf.Abs(width), Mathf.Abs(height));
    }
    private void FixesSizeBound(){
       Vector2 diferenceRect = rectBase - GetCameraRectangle();
       boundSize = new Vector3 (MathF.Abs(boundBase.x + diferenceRect.x + offset_toFixesSize.x), MathF.Abs(boundBase.y + diferenceRect.y + offset_toFixesSize.y), coll.size.z);
       coll.size = new Vector3 (MathF.Abs(boundSize.x), boundSize.y*(coll.size.y/MathF.Abs(boundSize.y)), coll.size.z);
    }
    
    private void ChangeSize(Vector2 pointMin, Vector2 pointMax) {
        if (coll == null)  return;

        var collBounds = coll.bounds;

        collBounds.min = pointMin;
        collBounds.max = pointMax;

        collBounds.size = new Vector2( Math.Abs(collBounds.size.x), Math.Abs(collBounds.size.y));

        var fixedXMinColl = collBounds.center.x - collBounds.size.x / 2;
        var fixedYMinColl = collBounds.center.y - collBounds.size.y / 2;

        var fixedXMaxColl = collBounds.center.x + collBounds.size.x / 2;
        var fixedYMaxColl = collBounds.center.y + collBounds.size.y / 2;

        var min = new Vector3(fixedXMinColl, fixedYMinColl);
        var max = new Vector3(fixedXMaxColl, fixedYMaxColl, -10);

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        bounds.size = new Vector3 (boundBase.x, bounds.size.y, 20);

        coll.center = bounds.center;      
        coll.size = new Vector3( Math.Abs( bounds.size.x), Math.Abs( bounds.size.y), Math.Abs( bounds.size.z));

        FixesSizeBound();
    }
}


[System.Serializable]
public class LimitsCamera{ public Vector2 min, max; }