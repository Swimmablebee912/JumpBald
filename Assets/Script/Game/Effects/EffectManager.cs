using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager main;
    private void Awake() { if(main!=null) Destroy(this.gameObject); else main=this; }

    public EffectsPool[] effects;
    public SettingsPopUP[] settingsPopUps;
    public SettingEffect[] settingsDust, settingTake;
    private EffectsPool currentEffect;
    
    public EffectManager InstanceEffect(string nameEffect, Vector3 position){
        currentEffect = new EffectsPool (GetEffect(nameEffect));
        GameObject newEffect = PoolManager.main.GetObject_InPool(TypePool.Effects, currentEffect.prefab);
        newEffect.transform.position = position;
        currentEffect.scrEffect = newEffect.GetComponent<EffectFunction>();
        return this;
    }
    public void DestroyEffect(GameObject object_) {
        PoolManager.main.HideObject(object_); 
    }
    
    public EffectManager SetDust(string text){
        foreach(SettingEffect setting in settingsDust){
            if(setting.name == text){
                currentEffect.scrEffect.SetColor(setting.GetColor());
                currentEffect.scrEffect.SetSpeed(setting.GetSpeed()); 
                currentEffect.scrEffect.SetPosition(setting.GetOffset());
                currentEffect.scrEffect.SetAngle(setting.GetOffsetAngle()); 
                currentEffect.scrEffect.SetScale(setting.GetScale()); 
            }
        }
        return this;
    }
    public EffectManager SetTakeVote(string text){
        foreach(SettingEffect setting in settingTake){
            if(setting.name == text){
                currentEffect.scrEffect.SetColor(setting.GetColor());
                currentEffect.scrEffect.SetSpeed(setting.GetSpeed()); 
                currentEffect.scrEffect.SetPosition(setting.GetOffset());
                currentEffect.scrEffect.SetAngle(setting.GetOffsetAngle()); 
                currentEffect.scrEffect.SetScale(setting.GetScale()); 
            }
        }
        return this;
    }
    public EffectManager SetPopUp(string text, TypePopUp type){
        currentEffect.scrEffect.SetText(text);
        foreach(SettingsPopUP setting in settingsPopUps){
            if(setting.type == type){
                currentEffect.scrEffect.SetColor(setting.GetColor());
                currentEffect.scrEffect.SetSpeed(setting.GetSpeed()); 
                currentEffect.scrEffect.SetPosition(setting.GetOffset());
                currentEffect.scrEffect.SetAngle(setting.GetOffsetAngle()); 
                currentEffect.scrEffect.SetScale(setting.GetScale()); 
            }
        }
        return this;
    }
    public EffectManager SetText(string text){currentEffect.scrEffect.SetText(text); return this;}
    public EffectManager SetColor(Color color){currentEffect.scrEffect.SetColor(color); return this;}
    public EffectManager SetSpeed(float speed){currentEffect.scrEffect.SetSpeed(speed); return this;}
    public EffectManager SetPosition(Vector3 offset){currentEffect.scrEffect.SetPosition(offset); return this;}
    public EffectManager SetRotation(float angle){currentEffect.scrEffect.SetAngle(angle); return this;}
    public EffectManager SetScale(Vector3 scale){currentEffect.scrEffect.SetScale(scale); return this;}

    private EffectsPool GetEffect(string nameEffect){
        foreach (EffectsPool effect_ in effects) {
            if(effect_.name==nameEffect)  return effect_;      
        }
        return null;
    }
}

[System.Serializable]
public class EffectsPool{
    public string name;
    public GameObject prefab;
    public EffectFunction scrEffect;

    public GameObject GetPrefab(){ return prefab; }

    public EffectsPool (EffectsPool copyEffect) {
        name=copyEffect.name;
        prefab=copyEffect.prefab;
        scrEffect=copyEffect.scrEffect;
    }
}

[System.Serializable]
public class SettingEffect{ 
    [Header("General")]
    public string name;
    
    public Color colorA;
    
    [Header("Configuration: Base")]
    public Vector3 offset;
    public float offsetAngle;
    public Vector3 scale;
    public float speed;

    [Header("Configuration: Randomizer")]
    public Vector3 randomOffset;
    public float randomOffsetAngle;
    public float randomScale;
    public float randomSpeed;


    public virtual Color GetColor(){
        return colorA;
    }
    public virtual Vector3 GetOffset(){
        return offset + new Vector3 (Random.Range(-randomOffset.x, randomOffset.x), Random.Range(-randomOffset.y, randomOffset.y), 0);
    }
    public virtual float GetOffsetAngle(){
        return offsetAngle +  Random.Range(-randomOffsetAngle, randomOffsetAngle);
    }
    public virtual Vector3 GetScale(){
        return scale + Vector3.one * Random.Range(-randomScale, randomScale);
    }
     public virtual float GetSpeed(){
        return speed +  Random.Range(-randomSpeed, randomSpeed);
    }

}

[System.Serializable]
public class SettingsPopUP:SettingEffect{ 
    public TypePopUp type;
}

public enum TypePopUp { action, damage, reward, rewardImportant, other}