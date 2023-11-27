using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    [HideInInspector] public Element scrThis;

    private void Start() { Initial(); }
    private void FixedUpdate() { Update_Fixed(); }
    private void Update() { Update_Check(); }
    private void LateUpdate() { Update_Apply();  }

    public virtual void Initial(){ }
    public virtual void Update_Fixed(){ }
    public virtual void Update_Check(){ }
    public virtual void Update_Apply(){ }

}
