using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MapManager : MonoBehaviour
{
    public static MapManager main; private void Awake()  { if(main==null) { main=this;} else { Destroy(gameObject); } }

    public DataStage dataStage;

    [System.Serializable] public class DataStage {

        [Header("Read")]
        public DataRoom currentRoom;

        public void ChangeRoom(DataRoom newCurrentRoom) { 
            if(newCurrentRoom != currentRoom) { 
                if(newCurrentRoom.data.stage != currentRoom.data.stage) { ShowUI_NewLevel(newCurrentRoom.data.stage.data.name); }
                currentRoom = newCurrentRoom;
                ShowUI_Room();
            }
        }
        private int Get_IdDataRoom() { int id = 0; foreach(DataRoom room in currentRoom.data.stage.configuration.rooms) { id++; if(currentRoom==room){ return id; } } return currentRoom.data.stage.configuration.rooms.Count; }
        

        private void ShowUI_NewLevel(string nameLevel){ CanvaManager.main.stage.ShowUI_NewLevel(nameLevel);}
        private void ShowUI_Room(){ CanvaManager.main.stage.ShowUI_Room(currentRoom.data.stage.data.name, Get_IdDataRoom().ToString()); }

    }
}

[System.Serializable] public class CheckpointData {  public int idScene; public Vector3 posCheckpoint; public RoomData dataRoom;  }   
[System.Serializable] public class RoomData { public int idScene; public string nameLevel; public int idRoom; public Room roomscr; }   
[System.Serializable] public class LevelData { public string name;  }   
