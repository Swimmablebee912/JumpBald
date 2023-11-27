using UnityEngine;

public class DataInteractable : ScriptableObject
{
    public Data data;
    
    [System.Serializable] public class Data {
        public string name; 
        [TextArea(2,5)] public string description;
        public Sprite illustration;
    }
}