using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jump Soul/Interactable/NPC")]
public class DataNPC : DataInteractable
{
    public Configuration configuration;
  
    [System.Serializable] public class Configuration {
        public List<Dialogue> dialogues;
    }
}
