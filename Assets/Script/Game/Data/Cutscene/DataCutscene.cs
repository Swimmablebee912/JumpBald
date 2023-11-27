using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jump Soul/Cutscene")]
public class DataCutscene : ScriptableObject
{
    public Data data;
    public Configuration configuration;

    [System.Serializable] public class Data{ 
        public string name;
        public List<Diapositive> diapositive;

        [System.Serializable] public class Diapositive{ 
            public string nameDiapositive = "???";
            [TextArea(2,5)] public string text = "Texto de ejemplo.";
            public Sprite illustration;
        }
    }

    [System.Serializable] public class Configuration{ 
        public string ToNextScene;
        public string ToNextSubScreen;
    }
    
}