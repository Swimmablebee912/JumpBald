using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jump Soul/Interactable/Checkpoint")]
public class DataCheckpoint : DataInteractable
{
    public Configuration configuration;
    
    [System.Serializable] public class Configuration {
        public DataRoom currentRoom;
        public Vector3 positionWorld;
    }
}
