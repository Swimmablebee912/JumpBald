using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UtilityElements : MonoBehaviour { }

[System.Serializable] public class CheckCollision {
    [Header("Configuration")]
    public Transform pivotCollision;
    
    public Vector3 offset;
    public float radius = .25f ;
    public LayerMask groundLayer = 1; 

    [Header("Read")]
    public bool inContact=false;
    public Collider2D coll;
    public Vector3 directionContact = Vector3.zero;
    public Vector3 posContact= Vector3.zero;

    public bool CheckEffector(Vector2 direction) { 
        coll = Physics2D.OverlapCircle(pivotCollision.position + offset, radius, groundLayer);
        if(coll!=null) {
            if(coll.usedByEffector) {
                PlatformEffector2D effect = coll.GetComponent<PlatformEffector2D>();
                if(effect != null) { 
                    Vector2 vectorAngle = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*effect.rotationalOffset), -Mathf.Cos(Mathf.Deg2Rad*effect.rotationalOffset)).normalized;
                    /*
                    if(Vector2.Dot((((Vector2)coll.transform.position) - (Vector2)(pivotCollision.position + offset)).normalized, vectorAngle) > 0) {
                        return inContact = true;
                    } else { return inContact = Vector2.Dot(direction.normalized, vectorAngle) >= 0f;  }
                    */
                    return inContact = Vector2.Dot(direction.normalized, vectorAngle) >= 0f;
                }
            } 
        }
        return inContact = coll!=null; 
    }
    public bool Check() { return inContact = (coll = Physics2D.OverlapCircle(pivotCollision.position + offset, radius, groundLayer))!=null; }
    public Vector2 GetClosePoint_Simple() { return posContact = Physics2D.OverlapCircle(pivotCollision.position + offset, radius, groundLayer).ClosestPoint(pivotCollision.position + offset);  }
    
}
[System.Serializable] public class CheckCollision_Cube {
    [Header("Configuration")]
    public Transform pivotCollision;
    
    public Vector3 offset;
    public Vector2Int size = Vector2Int.one ;
    public float angle = 0;
    public LayerMask groundLayer = 1; 

    [Header("Read")]
    public bool inContact=false;
    public Collider2D coll;
    public Vector2 directionContact = Vector2.zero;

    public CheckCollision_Cube(CheckCollision_Cube collisionInteract)
    {
        pivotCollision = collisionInteract.pivotCollision;
        offset = collisionInteract.offset;
        size = collisionInteract.size;
        angle = collisionInteract.angle;
        groundLayer = collisionInteract.groundLayer;
    }


    public bool Check() { return inContact = (coll = Physics2D.OverlapBox(pivotCollision.position + offset, size * (Vector2.one * .32f), angle, groundLayer))!=null; }
    public Vector2 GetClosePoint_Simple() { return Physics2D.OverlapBox(pivotCollision.position + offset, size * (Vector2.one * .32f), angle, groundLayer).ClosestPoint(pivotCollision.position + offset); }
}
[System.Serializable] public class SystemElement {
    [HideInInspector] public Element scrElement;

    public virtual void Initial ( Element scr ) { scrElement=scr; }
    public virtual void CheckFixed () { }
    public virtual void Check () {  }
    public virtual void Apply() {  }

}
[System.Serializable] public class Dialogue{ 
    public DataDialogue dataDialogue;
    public DataTimes dataTime;

    [System.Serializable] public class DataDialogue{ 
        public string nameChar = "???";
        public bool isCharacter = true;
        [TextArea(2,5)] public string dialogue = "Texto de ejemplo.";
        public Sprite face;
    }
    [System.Serializable] public class DataTimes{ 
        public float speedChar = 1f;
        public float speedSpace = .5f;
    }
}
