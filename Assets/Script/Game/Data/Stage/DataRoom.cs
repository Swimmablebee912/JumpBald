using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Jump Soul/Stage/Room")]
public class DataRoom : ScriptableObject {
    public Data data;
    public Configuration configuration;

    public int GetIdRoom() {
        int id = 0;
        foreach(DataRoom room in data.stage.configuration.rooms) { id++; if( room==this ) { return id; } }
        return id;
    }

    public List<string> GetScenes() { return new List<string> (configuration.scenes); }

    [System.Serializable] public class Data {
        public DataLevel stage;
        public string name; 
        public bool hideRoom;
    }
    [System.Serializable] public class Configuration {
        public int idScene;
        public List<string> scenes;
    }
    
}