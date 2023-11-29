using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main; private void Awake()  { if(main==null) { main=this;} else { Destroy(gameObject); } InAwake();}

    public SystemController systemManager;
    public SystemCutscene systemCutscene;
    public SystemRun systemRun;
    public SystemTimer systemTimer;
    public SystemInteract systemInteract;
    public SystemData systemData;

    private void Start() { InInitial();  }
    private void FixedUpdate() { InFixed(); }
    private void Update() { InCheck();  }
    private void LateUpdate() { InLate(); }
    private void OnApplicationQuit() { InQuit(); }
    private void OnApplicationFocus (bool hasfocus)  { InFocus(hasfocus); }

    private void InAwake(){ }
    private void InInitial() { main.StartCoroutine(StartedCoroutine());  }
    private void InFixed() { if(!systemManager.IsPlaying()) return; }
    private void InCheck() { if(!systemManager.IsPlaying()) return; systemTimer.UpdateTimer(); }
    private void InLate() { if(!systemManager.IsPlaying()) return; }
    private void InEndGame(){ systemRun.SaveRun(); }
    private void InQuit(){ systemRun.SaveRun(); }
    private void InFocus(bool inFocus) { if(inFocus) { systemRun.SaveRun(); } else {  } }

    private IEnumerator StartedCoroutine() {
        
        systemRun.LoadRun(SaveLoadController.main.GetData_CurrentSave());
        AudioManager.main.PlayMUSIC("Stop");

        if(systemRun.dataRun.positionWorld != Vector3.zero) { 
            SceneController.main.SetActiveScene(systemRun.dataRun.currentRoom);
            Player.main.System_state.dataRead.name = systemRun.dataRun.name; 
            Player.main.LoadData(); 
            yield return new WaitForSecondsRealtime(.25f);
            systemTimer.PauseTimer(false);
            CanvaManager.main.life.Show_Health(Player.main.System_state.dataRead.health, Player.main.System_state.dataConfiguration.healthMax);
            CanvaManager.main.stage.ShowUI_Room(systemRun.dataRun.currentRoom.data.stage.data.name, systemRun.dataRun.currentRoom.GetIdRoom().ToString());
            
        } else {
            Function_Button_CutsceneStart(systemCutscene.initial);
            yield return new WaitForSecondsRealtime(1.25f);
            systemTimer.TimerStart();
        }
    }

    public bool IsPlaying(){ return systemManager.IsPlaying(); }

    public void Function_Button_CutsceneStart(DataCutscene newData) { GameManager.main.system_GameFlow.ChangeSubScene("Cutscene"); systemCutscene.StartCutscene(newData); } 
    public void Function_Button_CutsceneNext() { systemCutscene.NextDiapositive(); }
    public void Function_Button_CutsceneEnd() { systemCutscene.EndCutscene(); }

    public void Function_Button_Interact() { Player.main.System_actions.Action_Interact(); }
    public void Function_Button_ChangeWeapon() { Player.main.System_actions.Action_ChangeWeapon(); }

    public void Function_Button_Revive() { 
        GameManager.main.system_GameFlow.ChangeSubScene("Interface"); 
        systemManager.Death(false);
        Player.main.System_actions.Action_Spawn();
    }

    public void Function_Button_FLowToResume () { systemManager.Pause(false); GameManager.main.system_GameFlow.ChangeSubScene("Interface"); }
    public void Function_Button_FLowToPause() { systemRun.SaveRun(); systemManager.Pause(true); GameManager.main.system_GameFlow.ChangeSubScene("Pause_Menu"); systemRun.ShowData_toPause(); }
    public void Function_Button_FLowToPauseOption() {  GameManager.main.system_GameFlow.ChangeSubScene("Pause_Option"); }
    public void Function_Button_FLowToDeath () { systemRun.SaveRun(); systemManager.Death(true); GameManager.main.system_GameFlow.ChangeSubScene("Death"); systemRun.ShowData_toDeath(); }

    public void Function_Button_FLowToRestart () { 
        SaveLoadController.main.SetData_CurrentSave( new RunData { name = systemRun.dataRun.name });
        GameManager.main.system_GameFlow.ChangeSceneAndSubSceneReload("Gameplay", "Interface"); 
    }
    public void Function_Button_FLowToMainMenu () { systemRun.SaveRun(); GameManager.main.system_GameFlow.ChangeSceneAndSubScene("MainMenu", "StartGame"); }
    
    [System.Serializable] public class SystemController {
        [Header("About State")]
        public bool inPause = false; public bool inDeath = false;

        [Header("Reference Canvas")]
        public Animator anim;

        public void Spawn(){  AudioManager.main.PlaySFX("SFX_UI_Stinger_Gameplay_Spawn"); }
        public void Pause(bool state) { 
            inPause = state; main.systemRun.ShowData_toPause(); 
            if(state) { AudioManager.main.PlaySFX("UI_Trigger_Gameplay_Pause_On"); } 
            else { AudioManager.main.PlaySFX("UI_Trigger_Gameplay_Pause_Off"); }
        }
        public void Death(bool state) { inDeath = state; main.systemRun.ShowData_toDeath(); AudioManager.main.PlaySFX("UI_Trigger_Gameplay_Death"); }

        public bool IsPlaying() { return !inPause && !inDeath; }
    }
    [System.Serializable] public class SystemRun {
        public RunData dataRun = new RunData ();
        
        public void ShowData_toDeath() {  if(dataRun!=null) CanvaManager.main.menu.menuDeath.ShowData(dataRun.name, dataRun.time, string.Format("{0} {1}", dataRun.currentRoom.data.stage.data.name, dataRun.currentRoom.GetIdRoom())); }
        public void ShowData_toPause() { if(dataRun!=null) CanvaManager.main.menu.menuPause.ShowData(dataRun.name, dataRun.time); }

        public void SaveRun() {
            dataRun.name = SaveLoadController.main.GetData_CurrentSave().name; 
            dataRun.life = Player.main.System_state.dataRead.health;
            dataRun.lifeMax =  Player.main.System_state.dataConfiguration.healthMax;
            if(Player.main.System_state.dataRead.achievements!=null) { 
                dataRun.achievements = new List<string>{}; 
                foreach(DataAchievement data in Player.main.System_state.dataRead.achievements){ dataRun.achievements.Add(data.name); }
            }
            if(Player.main.System_state.dataRead.weapons!=null) { 
                dataRun.weapons = new List<string>{}; 
                foreach(DataWeapon data in Player.main.System_state.dataRead.weapons){ dataRun.weapons.Add(data.name); }
            }
            if(Player.main.System_state.dataRead.checkpoints!=null) { 
                dataRun.checkpoints = new List<string>{}; 
                foreach(DataCheckpoint data in Player.main.System_state.dataRead.checkpoints){ dataRun.checkpoints.Add(data.name); }
            }
            dataRun.time = main.systemTimer.time;
            dataRun.idScene = dataRun.currentRoom.configuration.idScene;

            SaveLoadController.main.Save(dataRun); 
        }
        public void LoadRun(RunData newRunData) { 
            dataRun = newRunData; 
        }
    }
    [System.Serializable] public class SystemTimer {
        [Header("Read")]
        public float time=0;
        public bool timeUse = false;

        public void UpdateTimer() { if(timeUse){ time+=Time.deltaTime; } ShowTimer(); }
        public void PauseTimer( bool state) { timeUse = !state; }
        public void ResetTimer() { timeUse=false; time=0; ShowTimer(); } 
        public void TimerStart() { timeUse=true; time=0; ShowTimer(); }

        public void ShowTimer(){ CanvaManager.main.timer.ShowTimer(time); }
    }
    [System.Serializable] public class SystemInteract{
        public SystemDialogue systemDialogue;
        public SystemCheckpoint systemCheckpoint;
        public SystemItem systemItem;

        [System.Serializable] public class SystemDialogue {
            [Header("Read")]
            public Interactable_NPC currentScrNPC;
            public Dialogue currentDialogue;

            public void Set_Dialogue(Interactable_NPC newNPC, Dialogue newDialogue) {
                currentScrNPC = newNPC;
                currentDialogue=newDialogue;

                CanvaManager.main.interaction.dialogue.ShowDialogue(newNPC, newDialogue);
            }
        }
        [System.Serializable] public class SystemCheckpoint {
            [Header("Read")]
            public Interactable_Checkpoint currentCheckpoint;

            public void Set_CheckpointInteract(Interactable_Checkpoint newCheckpoint) {
                currentCheckpoint = newCheckpoint;
                CanvaManager.main.interaction.checkpoint.ShowCheckpoint(currentCheckpoint);
            }
        }
        [System.Serializable] public class SystemItem {
            [Header("Read")]
            public Interactable_Item currentItem;

            public void Set_ItemInteract(Interactable_Item newItem) {
                currentItem = newItem;
                CanvaManager.main.interaction.item.ShowItem(currentItem);
            }
        }
    }
    [System.Serializable] public class SystemCutscene{
        [Header("Configuration")]
        public DataCutscene initial;
        public DataCutscene win;

        [Header("Read")]
        public DataCutscene currentData;
        public int countId;

        public void StartCutscene(DataCutscene newData) {
            countId = 0;
            currentData=newData;
            ShowDiapositive();
        }
        public void NextDiapositive() {
            countId ++; 
            if(countId >= currentData.data.diapositive.Count) { EndCutscene(); }
            else { ShowDiapositive(); }
        }
        public void EndCutscene(){
            if(currentData.configuration.ToNextSubScreen != "") { GameManager.main.system_GameFlow.ChangeSceneAndSubScene(currentData.configuration.ToNextScene, currentData.configuration.ToNextSubScreen); } 
            else if(currentData.configuration.ToNextScene != "") { GameManager.main.system_GameFlow.ChangeScene(currentData.configuration.ToNextScene); }
        }

        public void ShowDiapositive() { CanvaManager.main.menu.menuCutscene.ShowData(currentData.data.diapositive[countId]); }
    }
    [System.Serializable] public class SystemData {
        public List<DataAchievement> dataAchievement;
        public List<DataWeapon> dataWeapons;
        public List<DataCheckpoint> dataCheckpoints;

        public DataAchievement GetAchievement(string name) { foreach(DataAchievement data in dataAchievement) { if(name==data.name) return data; } return null; }
        public DataWeapon GetWeapon(string name) { foreach(DataWeapon data in dataWeapons) { if(name==data.name) return data; } return null; }
        public DataCheckpoint GetCheckpoint(string name) { foreach(DataCheckpoint data in dataCheckpoints) { if(name==data.name) return data; } return null; }
    }
}
