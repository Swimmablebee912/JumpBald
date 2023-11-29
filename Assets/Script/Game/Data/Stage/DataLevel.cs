using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jump Soul/Stage/Level")]
public class DataLevel : ScriptableObject
{
    public Data data;
    public Configuration configuration;
    
    [System.Serializable] public class Data {
        public string name;
        [TextArea(2,5)] public string description;
        public string musicLevel = "";
    }
    [System.Serializable] public class Configuration {
        public List<DataRoom> rooms;
    }
}