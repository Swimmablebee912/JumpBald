using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    private Transform transf;

    public virtual void Awake(){ transf = transform; }

    public virtual ObjectPool InPool_Active( ) {
        return this;
    }
    public virtual ObjectPool InPool_Inactive() {
        StopAllCoroutines();
        return this;
    }

    public virtual ObjectPool Set_Transform( Vector3 position, Quaternion rotation) { 
        transf.position = position;
        transf.rotation = rotation;
        return this;
    }
    public virtual ObjectPool Set_Parent( Transform parent) {
        transf.SetParent(parent);
        return this;
    }
}
