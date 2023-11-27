using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController main; private void Awake()  { if(main==null) { main=this;} else { Destroy(gameObject); } }
    
    [Header("Read")]
    public DataRoom currentRoom;

    public string[] sceneActived_string;
    public string[] sceneAssets_string;

    private void Start() { Initial_SceneLoad(); }
    private void Initial_SceneLoad(){ SetActiveScene(currentRoom); }

    public void SetActiveScene(DataRoom room) {
        currentRoom = room; 
        sceneAssets_string = new List<string> (currentRoom.GetScenes()).ToArray();

        foreach (string item in sceneActived_string) {
            bool isFinded = false;
            foreach(string itemSub in sceneAssets_string) { if(itemSub == item) { isFinded = true;  break; } }
            if (!isFinded) if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(item).isLoaded) { UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(item); }
        }
        foreach (string item2 in sceneAssets_string) {
            bool isFinded = false;
            foreach(string itemSub in sceneActived_string) { if(itemSub == item2) { isFinded = true;  break; } }
           if (!isFinded) { if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName(item2).isLoaded){ UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(item2, LoadSceneMode.Additive); } }
        }

        sceneActived_string = new List<string>(sceneAssets_string).ToArray();
    }    
}

