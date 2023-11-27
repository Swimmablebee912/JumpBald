using UnityEngine;

[CreateAssetMenu(menuName = "Jump Soul/Interactable/Item")]
public class DataItem : DataInteractable
{
    public Configuration configuration;
  
    [System.Serializable] public class Configuration {
        public TypeItem type; public enum TypeItem { Life }
        public int count = 1;
    }
}
