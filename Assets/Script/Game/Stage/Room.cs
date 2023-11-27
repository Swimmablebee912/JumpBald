using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    [Header("Reference")]
    public BoxCollider2D coll;
    public Collider confinder;

    [Header("Configuration")]
    public DataRoom data;
    public List<Interactable> interactablesRoom;
    public List<Obstacle> obstaclesRoom;


    [Header("Read")]
    public bool inContact = false;

    private void Update() { CheckState(); }
    
    private bool CheckCollider () { return coll.OverlapPoint(Player.main.System_gravity.dataReference.pivot.position);  }
    private void CheckState() { if(inContact!=CheckCollider()) { inContact = CheckCollider(); if(inContact) ActiveScene(); }  }

    private void ActiveScene(){  
        SceneController.main.SetActiveScene(data); 
        MapManager.main.dataStage.ChangeRoom(data);
        Player.main.System_gravity.SetCurrentRoom(data);
        CameraManager.main.SetCameraConfinder(confinder.bounds); 
        CheckObjects();
    }

    private void CheckObjects(){ 
        foreach (Interactable interactableRoom in interactablesRoom) { interactableRoom.Check_StateInteractable(); }
        foreach (Obstacle obstacleRoom in obstaclesRoom) { obstacleRoom.Reset_Initial(); }
    }
    
}
