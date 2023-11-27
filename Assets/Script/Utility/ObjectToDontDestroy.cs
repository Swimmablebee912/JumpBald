using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToDontDestroy : MonoBehaviour
{
    public void OnEnable() { DontDestroyOnLoad(this); }
}
