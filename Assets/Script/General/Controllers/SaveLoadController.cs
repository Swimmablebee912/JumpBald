using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadController : MonoBehaviour
{
    public static SaveLoadController main; private void Awake()  { if(main==null) { main=this; Initial(); } else { Destroy(gameObject); }}

    [Header("Configuration")]
    public string savePath = "/RunData.dat"; private string path;
    public int countSaves = 4;
    //private string encryptionKey = "YourEncryptionKey";

    [Header("Read")] 
    public Runs data;
    private RunData currentRun;

    public List<RunData> GetRuns() { data = Load(); return data.dataList;   }

    private void Initial() { path = Application.persistentDataPath + savePath;  }
    
    public RunData GetData_CurrentSave () { return  currentRun; }
    public void SetData_CurrentSave (RunData currentSave) { currentRun = currentSave; }

    public Runs Load() {
        if (File.Exists(path))  {
            // BinaryFormatter formatter = new BinaryFormatter();
            // FileStream fileStream = File.Open(path, FileMode.Open);
            // Runs runs = (Runs)formatter.Deserialize(fileStream);
            // fileStream.Close();
            // runs = DecryptData(runs);
            // return data = runs;
            string json = File.ReadAllText(path);
            Runs runs =  new Runs {}; bool eliminatedRun=false;
            foreach(RunData data in JsonUtility.FromJson<Runs>(json).dataList){ if(data.life>0) { runs.dataList.Add(data); eliminatedRun=true; } }
            data = runs;
            if(eliminatedRun) SaveData(runs);
            return data;
        }
        else { return new Runs(); }
    }
    public void Save( RunData newSave) { 
        Runs runs = Load();
        
        RunData existingRun = runs.dataList.Find(run => run.name == newSave.name);

        if (existingRun != null)  { 
            existingRun.name = newSave.name;
            existingRun.life = newSave.life; 
            existingRun.lifeMax = newSave.lifeMax;
            existingRun.achievements = newSave.achievements;
            existingRun.weapons = newSave.weapons;
            existingRun.checkpoints = newSave.checkpoints;
            existingRun.currentRoom = newSave.currentRoom;
            existingRun.positionWorld = newSave.positionWorld;
            existingRun.time = newSave.time;
        } 
        else  { runs.dataList.Add(newSave);  }

        SaveData(runs);
    }
    private void SaveData(Runs runs) {
        data = runs;
        // string encryptedData = EncryptData(JsonUtility.ToJson(runs));
        // File.WriteAllText(path, encryptedData);
        string json = JsonUtility.ToJson(runs);
        File.WriteAllText(path, json);
    }

    private string EncryptData(string data)  {
        // char[] dataChars = data.ToCharArray();
        // char[] keyChars = encryptionKey.ToCharArray();
        // for (int i = 0; i < dataChars.Length; i++)  { dataChars[i] = (char)(dataChars[i] ^ keyChars[i % keyChars.Length]);  }
        // return new string(dataChars);
        return data;
    }
    private Runs DecryptData(Runs runs)  {
        // string encryptedData = JsonUtility.ToJson(runs);
        // char[] encryptedDataChars = encryptedData.ToCharArray();
        // char[] keyChars = encryptionKey.ToCharArray();

        // for (int i = 0; i < encryptedDataChars.Length; i++)  { encryptedDataChars[i] = (char)(encryptedDataChars[i] ^ keyChars[i % keyChars.Length]); }

        // string decryptedData = new string(encryptedDataChars);
        // return JsonUtility.FromJson<Runs>(decryptedData);
        return runs;
    }
}

[System.Serializable] public class Runs {
    public List<RunData> dataList = new List<RunData> { };
}
[System.Serializable] public class RunData {

    [Header("About Character")]
    public string name = "Unnamed";
    public int life = 3; public int lifeMax = 3;
    public List<DataAchievement> achievements = null;
    public List<DataWeapon> weapons = null;
    public List<DataCheckpoint> checkpoints = null;

    [Header("About Stage")]
    public DataRoom currentRoom = null;
    public Vector3 positionWorld = Vector3.zero;

    [Header("About Game")]
    public float time = 0;

    public void SetGeneral(RunData data) { 
        SetData_Name(data.name);
        SetData_Life(data.life);
        SetData_Life(data.lifeMax);
        SetData_Achievement(data.achievements);
        SetData_Weapon(data.weapons);
        SetData_Checkpoint(data.checkpoints);
        SetData_Room(data.currentRoom);
        SetData_PositionWorld(data.positionWorld);
        SetData_Time(data.time);
    }

    public void SetData_Name(string newValue) { name = newValue; }
    public void SetData_Life(int newValue) { life = newValue; }
    public void SetData_LifeMax(int newValue) { lifeMax = newValue; }
    public void SetData_Achievement(List<DataAchievement> newValue) { achievements = newValue; }
    public void SetData_Weapon(List<DataWeapon> newValue) { weapons = newValue; }
    public void SetData_Checkpoint(List<DataCheckpoint> newValue) { checkpoints = newValue; }
    public void SetData_Room(DataRoom newValue) { currentRoom = newValue; }
    public void SetData_PositionWorld(Vector3 newValue) { positionWorld = newValue; }
    public void SetData_Time(float newValue) { time = newValue; }
}
    