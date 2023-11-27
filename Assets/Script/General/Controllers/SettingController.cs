using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SettingController : MonoBehaviour
{   
    public static SettingController main;
    private void Awake() { if(main==null) main=this; else Destroy(gameObject); }

    public string pathResource = "/DatosSetting.json";

    public SettingsController setting;


    public void LoadSetting() { 

        string rutaArchivo = Application.persistentDataPath + pathResource;

        if (File.Exists(rutaArchivo)) {
            string datosJson = File.ReadAllText(rutaArchivo);
            setting = JsonUtility.FromJson<SettingsController>(datosJson);
            setting.setting_Audio.muted=false;
        } else {
            SaveSetting( 
                new SettingData_Audio () { muted = true },
                new SettingData_Input () { invert = false }
            );
        }

        setting.LoadSetting(); 
    }
    public SettingData_Audio LoadSetting_Audio() { LoadSetting(); return setting.setting_Audio; }
    
    public void SaveSetting( SettingData_Audio newSettingAudio, SettingData_Input newSettingInput) { 
        setting.SaveSetting(newSettingAudio); 
        setting.SaveSetting(newSettingInput); 

        string datosJson = JsonUtility.ToJson(setting);
        string rutaArchivo = Application.persistentDataPath + pathResource;
        File.WriteAllText(rutaArchivo, datosJson);
    }
    public void SaveSetting( SettingData_Audio newSetting) { 
        setting.SaveSetting(newSetting); 

        string datosJson = JsonUtility.ToJson(setting);
        string rutaArchivo = Application.persistentDataPath + pathResource;
        File.WriteAllText(rutaArchivo, datosJson);
    }
    public void SaveSetting( SettingData_Input newSetting) { 
        setting.SaveSetting(newSetting); 

        string datosJson = JsonUtility.ToJson(setting);
        string rutaArchivo = Application.persistentDataPath + pathResource;
        File.WriteAllText(rutaArchivo, datosJson);
    }

}

[System.Serializable] public class SettingsController {
        
    public SettingData_Audio setting_Audio; 
    public SettingData_Input setting_Input; 

    public void LoadSetting() { 
        setting_Audio.ShowSetting(); 
        setting_Input.ShowSetting(); 
    }

    public void SaveSetting( SettingData_Audio newAudio ) { 
        setting_Audio = newAudio;
        setting_Audio.ShowSetting();
    }
    public void SaveSetting( SettingData_Input newInput ) { 
        setting_Input = newInput;
        setting_Input.ShowSetting();
    }
}

[System.Serializable] public class SettingData_Audio { 
    public bool muted;  
    public void ShowSetting() {   }
}
[System.Serializable] public class SettingData_Input { 
    public bool invert;  
    public void ShowSetting() {  }
}
