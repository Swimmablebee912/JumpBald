using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jump Soul/Interactable/Weapon")]
public class DataWeapon : DataInteractable
{
    public Configuration configuration;
  
    [System.Serializable] public class Configuration {
        public Sprite sprWeapon;
        public TypeAction type; public enum TypeAction { None, Stopped, WallCling, Jump }
    }
}