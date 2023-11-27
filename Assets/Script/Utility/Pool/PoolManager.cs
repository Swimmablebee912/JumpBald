using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager main;
    private void Awake() { if(main!=null) Destroy(this.gameObject); else main=this; }

    [SerializeField] List<Pool> pools;
    [SerializeField] public Transform parentPool, parentSpawn;
    GameObject_InPool newObject, findedObject;

    private void Start() { Initializer(); }
    void Initializer() {foreach (Pool pool in pools) { pool.Initialize(); }  }
    
    public GameObject GetObject_InPool(TypePool typePool, GameObject reference) {
        Pool findedPool = System.Array.Find(pools.ToArray(), pool => pool.type == typePool);
        if(findedPool!=null) {
            newObject = findedPool.GetObject(reference);
            if(newObject!=null){
                newObject.gameobject.SetActive(true);
                newObject.objectPool.InPool_Active();
                newObject.objectPool.Set_Parent(null);
                return newObject.gameobject;
            }
        }
        return null;
    }
    public void HideObject(GameObject reference) {
        foreach (Pool pool in pools) {
            newObject = pool.GetObject(reference);
            if (newObject != null) {
                reference.transform.position = Vector3.down*100;
                newObject.objectPool.InPool_Inactive();
                reference.transform.SetParent(parentPool);
                reference.SetActive(false);
                return;
            }
        }
      
    }
    
    public void HideAllObjects(){ foreach (Pool pool in pools) { pool.HideAllObjects(); } }

    public void DestroyAllObjects(){
        foreach (Pool pool in pools) { 
            List<GameObject_InPool> objects = pool.GetAllObjects();
            foreach(GameObject_InPool object_ in objects){ Destroy(object_.gameobject); }
        }
        foreach (Pool pool in pools) { pool.ClearAllObjects(); }
    }
 }

[System.Serializable]
public class Pool {
    public string namePath;
    public TypePool type;
    public List<GameObject> prefabs;
    [Range(0, 50)] public int countForCreate;
    [SerializeField] public List<GameObject_InPool> objects;
    
    private GameObject newObject;

    public void Initialize(){
        foreach (GameObject prefab in prefabs) {
            for (int i = 0; i < countForCreate; i++) {
                newObject = PoolManager.Instantiate(prefab);
                newObject.name = newObject.name.Replace("(Clone)","");

                GameObject_InPool newObj = AddObject(newObject);
                newObj.gameobject.SetActive(false);
                newObj.objectPool.Set_Parent(PoolManager.main.parentPool);
            }
        }
    }
    public GameObject_InPool CreateInPool(GameObject toCreate){

        findedObject = Resources.Load<GameObject>(namePath +"/"+ toCreate.name);

        if(findedObject!=null){

            foreach(GameObject prefab in prefabs){if(prefab.name==findedObject.name) findedInPrefabs=true; }
            if(!findedInPrefabs) prefabs.Add(findedObject);

            GameObject_InPool newObj = null;

            for (int i = 0; i < countForCreate; i++) {
                newObject = PoolManager.Instantiate(findedObject);
                newObject.name = newObject.name.Replace("(Clone)","");
                
                newObj = AddObject(newObject);
                newObj.gameobject.transform.position = Vector3.down*100;
                newObj.gameobject.SetActive(false);
                newObj.objectPool.Set_Parent(PoolManager.main.parentPool);
                
            }
            
            return newObj;
        }
        
        return null;
    }

    public void HideAllObjects() { 
        foreach(GameObject_InPool obj in objects){
            obj.objectPool.InPool_Inactive();
            obj.objectPool.Set_Parent(PoolManager.main.parentPool);
            obj.gameobject.SetActive(false);
        } 
    }

    public GameObject_InPool AddObject(GameObject newGameobject) { 
        GameObject_InPool newObj = new GameObject_InPool{
            name= newGameobject.name,
            gameobject = newGameobject,
            objectPool = newGameobject.GetComponent<ObjectPool>() };
        objects.Add(newObj); 
        return newObj; 
    }
    public GameObject RemoveObject(GameObject newGameobject) { 
        GameObject_InPool newObj = System.Array.Find(objects.ToArray(), objectFinded => objectFinded.gameobject == newGameobject);
        if(newObj!=null) objects.Remove(newObj); 
        return newGameobject; 
    }

    public List<GameObject_InPool> GetAllObjects() { return objects; }
    public void ClearAllObjects() { objects.Clear(); }

    private GameObject findedObject;
    private bool findedInPrefabs;
    public GameObject_InPool GetObject(GameObject reference) {
        findedObject = reference;

        foreach (GameObject_InPool obj_ in objects) {
            if(obj_.name == findedObject.name){ 
                    if (!obj_.gameobject.activeSelf) { return obj_; } 
                }
        }
        return CreateInPool(findedObject);
    }
}

[System.Serializable]
public class GameObject_InPool {
    public string name;
    public GameObject gameobject;
    public ObjectPool objectPool;
}

public enum TypePool { Entity, Element, Effects, Other, Stage }