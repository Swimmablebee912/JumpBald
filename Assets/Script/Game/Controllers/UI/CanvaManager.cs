using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CanvaManager : MonoBehaviour
{
    public static CanvaManager main; private void Awake()  { if(main==null) { main=this;} else { Destroy(gameObject); } }

    public VisualTimer timer;
    public VisualStage stage;
    public VisualLife life;
    public VisualWeapon weapon;
    public VisualInteraction interaction;
    public VisualNotification notification;
    public VisualMenu menu;

    [System.Serializable] public class VisualTimer {
        [Header("Reference")]
        public TextMeshProUGUI textTimer;

        [Header("Configuration")]
        public string timerText = "<color=#fff><size=150%>{0:00}</size></color>:<color=#fff><size=150%>{1:00}</size></color>:<size=70%>{2:00}</size>";

        public void ShowTimer(float time) { textTimer.text = string.Format(timerText, Mathf.FloorToInt(time / 60), Mathf.FloorToInt(time % 60),  Mathf.FloorToInt((time * 1000000) % 100)); }
        public string GetTimer() { return textTimer.text;  }
    }
    [System.Serializable] public class VisualStage {
        [Header("Reference")]
        public TextMeshProUGUI textRoom;
        public TextMeshProUGUI textLevel;

        [Header("Configuration")]
        public AnimationText textAnimation;

        public void ShowUI_NewLevel(string currentLvl){
            textLevel.DOKill(false);
            textLevel.gameObject.SetActive(true);
            textLevel.color=textAnimation.colorTextLevel_Inactive;
            textLevel.SetText(currentLvl); 
            textLevel.DOColor(textAnimation.colorTextLevel_Active, textAnimation.timeIntro).OnComplete(()=> { 
                textLevel.DOColor(textAnimation.colorTextLevel_Inactive, textAnimation.timeExit).SetDelay(textAnimation.timeStay).OnComplete(()=>{
                    textLevel.gameObject.SetActive(false);
                });
            });
        }
        public void ShowUI_Room(string nameLevel, string idRoom){ textRoom.SetText(string.Format("{0}<size=150%> <color=#fff>{1}", nameLevel, idRoom));  }

        [System.Serializable] public class AnimationText { public float timeIntro, timeStay, timeExit; public Color colorTextLevel_Active, colorTextLevel_Inactive; } 
    }
    [System.Serializable] public class VisualLife {
        [Header("Reference")]
        public TextMeshProUGUI textHealth;
        public Transform transformBox;

        public void Show_Health(int value, int max) { 
            string text = "";
            for(int i=1; i<=max ;i++){ text += i<=value?"<color=#fff><sprite=0></color>":"<sprite=1>"; }
            textHealth.SetText(text); 
            transformBox.DOKill(false); transformBox.localScale = new Vector3(1,1,1);
            transformBox.DOShakeScale(0.1f).OnComplete(()=> { transformBox.localScale = new Vector3(1,1,1); });
        }
    }
    [System.Serializable] public class VisualWeapon {
        [Header("Reference")]
        public Image image;

        public void Show_Weapon(Sprite illustrationWeapon) { image.sprite = illustrationWeapon; }
    }
    [System.Serializable] public class VisualInteraction {
        public VisualItem item;
        public VisualDialogue dialogue;
        public VisualCheckpoint checkpoint;

        [System.Serializable] public class VisualCheckpoint {
            [Header("Reference")]
            public Transform transformBox;
            public TextMeshProUGUI textName;
            public Image imgBackground;
            public Image imgFace;

            [Header("Read")]
            public bool isActive;
            public Interactable_Checkpoint currentScrCheckpoint;

            public void ShowCheckpoint(Interactable_Checkpoint newCheckpoint) {
                if( isActive != (newCheckpoint!=null)) {  isActive = newCheckpoint!=null; ShowBoxItem(isActive);  }
                currentScrCheckpoint = newCheckpoint;
                if(newCheckpoint!=null) {
                    ShowDataRoom(currentScrCheckpoint.interaction.data.configuration.currentRoom.data.stage.data.name, currentScrCheckpoint.interaction.data.configuration.currentRoom.GetIdRoom().ToString());
                    ShowFace(currentScrCheckpoint.interaction.data.data.illustration);
                    ShowActive(currentScrCheckpoint.interaction.isActive);
                }
            }
            private void ShowBoxItem(bool show) {
                transformBox.DOKill(false);
                if(show) {
                    transformBox.gameObject.SetActive(true);
                    transformBox.DOScale(Vector3.one, 0.1f);
                } else {
                    transformBox.DOScale(Vector3.zero, 0.1f).OnComplete(()=>{ transformBox.gameObject.SetActive(false); });
                }
            }
            private void ShowActive(bool isActive) { 
                if(isActive) { 
                    imgBackground.color=Color.white;
                    textName.color=Color.black;
                }
                else {
                    imgBackground.color=Color.black;
                    textName.color=Color.white;
                }
            }
            private void ShowDataRoom(string nameLevel, string nameRoom) { textName.SetText(string.Format("Level {0}\nRoom {1}", nameLevel, nameRoom)); }
            private void ShowFace( Sprite spr) { imgFace.sprite = spr; }
        }
        [System.Serializable] public class VisualItem {
            [Header("Reference")]
            public Transform transformBox;
            public TextMeshProUGUI textName;
            public TextMeshProUGUI textDialogue;
            public Image imgFace;

            [Header("Configuration")]
            public float timeChar = 1;

            [Header("Read")]
            public bool isActive;
            public Interactable_Item currentScrItem;

            public void ShowItem(Interactable_Item newItem) {
                if( isActive != (newItem!=null)) {  isActive = newItem!=null; ShowBoxItem(isActive);  }
                if (currentScrItem != newItem) {
                    currentScrItem = newItem;
                    if(textCoroutine!=null) { CanvaManager.main.StopCoroutine(textCoroutine); }
                    if(newItem!=null) {
                        switch(newItem.interaction.typeItem) {
                            case Interactable_Item.Interact.TypeItem.Item: 
                                ShowName(newItem.interaction.item.data.name);
                                ShowTextDescription(newItem.interaction.item.data.description);
                                ShowFace(newItem.interaction.item.data.illustration);
                                break;
                            case Interactable_Item.Interact.TypeItem.Achievement: 
                                ShowName(newItem.interaction.achievement.data.name);
                                ShowTextDescription(newItem.interaction.achievement.data.description);
                                ShowFace(newItem.interaction.achievement.data.illustration);
                                break;
                            case Interactable_Item.Interact.TypeItem.Weapon: 
                                ShowName(newItem.interaction.weapon.data.name);
                                ShowTextDescription(newItem.interaction.weapon.data.description);
                                ShowFace(newItem.interaction.weapon.data.illustration);
                                break;
                        }
                        
                    }
                }
                
            }
            private void ShowBoxItem(bool show) {
                transformBox.DOKill(false);
                if(show) {
                    transformBox.gameObject.SetActive(true);
                    transformBox.DOScale(Vector3.one, 0.1f);
                } else {
                    transformBox.DOScale(Vector3.zero, 0.1f).OnComplete(()=>{ transformBox.gameObject.SetActive(false); });
                }
            }

            private void ShowName(string nameChar) { textName.SetText(nameChar); }
            private void ShowTextDescription(string dialogue) { 
                textCoroutine = CanvaManager.main.StartCoroutine(WriteText(dialogue));
            } 
            private Coroutine textCoroutine;
            IEnumerator WriteText(string text) { 
                string textWritter = ""; textDialogue.SetText("");
                foreach (char c in text) {
                    textWritter += c; textDialogue.SetText(textWritter);
                    yield return new WaitForSeconds(timeChar);
                }
            }
            private void ShowFace( Sprite spr) { imgFace.sprite = spr; }
        }
        [System.Serializable] public class VisualDialogue {
            [Header("Reference")]
            public Transform transformBox;
            public TextMeshProUGUI textName;
            public TextMeshProUGUI textDialogue;
            public Image imgBackground;
            public Image imgFace;

            [Header("Configuration")]
            public float timeChar = 1;
            
            [Header("Read")]
            public bool isActive;
            public Interactable_NPC currentScrNPC;
            public Dialogue currentDialogue;
            public bool currentIsCharacter;
            public string currentNameChar;
            public string currentTextDialogue;
            public Sprite currentFace;

            public void ShowDialogue(Interactable_NPC newNPC, Dialogue newDialogue) {
                if( isActive != (newDialogue!=null)) {  isActive = newDialogue!=null; ShowBoxDialogue(isActive);  }

                currentScrNPC = newNPC;
                currentDialogue=newDialogue;

                if(textCoroutine!=null) { CanvaManager.main.StopCoroutine(textCoroutine); }

                if(newNPC!=null) {
                    currentNameChar=newDialogue.dataDialogue.nameChar; ShowName(currentNameChar);
                    currentTextDialogue=newDialogue.dataDialogue.dialogue; ShowTextDialogue(currentTextDialogue, newDialogue.dataDialogue.isCharacter);
                    currentFace=newDialogue.dataDialogue.face; ShowFace(currentFace);
                    currentIsCharacter=newDialogue.dataDialogue.isCharacter; ShowTypeDialogue(currentIsCharacter);
                }
            }

            private void ShowBoxDialogue(bool show) {
                transformBox.DOKill(false);
                if(show) {
                    transformBox.gameObject.SetActive(true);
                    transformBox.DOScale(Vector3.one, 0.1f);
                } else {
                    transformBox.DOScale(Vector3.zero, 0.1f).OnComplete(()=>{ transformBox.gameObject.SetActive(false); });
                }
            }

            private void ShowTypeDialogue(bool isCharacter) { 
                if(isCharacter) { 
                    imgBackground.color=Color.white;
                    textDialogue.color=Color.black;
                    textName.color=Color.black;
                }
                else {
                    imgBackground.color=Color.black;
                    textDialogue.color=Color.white;
                    textName.color=Color.white;
                }
            }
            private void ShowName(string nameChar) { textName.SetText(nameChar); }
            private void ShowTextDialogue(string dialogue, bool isCharacter) { 
                if(isCharacter) textCoroutine = CanvaManager.main.StartCoroutine(WriteText(dialogue));
                else textDialogue.SetText(dialogue);
            } 
            
            private Coroutine textCoroutine;
            IEnumerator WriteText(string text) { 
                string textWritter = ""; textDialogue.SetText("");
                foreach (char c in text) {
                    textWritter += c; textDialogue.SetText(textWritter);
                    yield return new WaitForSeconds(timeChar * (char.IsWhiteSpace(c) ? currentDialogue.dataTime.speedChar : currentDialogue.dataTime.speedChar));
                }
            }
            private void ShowFace( Sprite spr) { imgFace.sprite = spr; }
        }
    }
    [System.Serializable] public class VisualNotification {
        [Header("Reference")]
        public Transform transformBox;
        public TextMeshProUGUI textName;

        public void ShowNotification(string text, float timer) {
            transformBox.DOKill(false);
            transformBox.localScale=new Vector3(1,0,1);
            transformBox.gameObject.SetActive(true);
            textName.SetText(text);
            transformBox.DOScale(Vector3.one, 0.1f).OnComplete(()=> { 
                transformBox.DOScale(new Vector3(1,0,1), 0.1f).SetDelay(timer).OnComplete(()=>{ transformBox.gameObject.SetActive(false); });
            });
        }
    }
    [System.Serializable] public class VisualMenu {
        public MenuPause menuPause;
        public MenuDeath menuDeath;
        public MenuCutscene menuCutscene;

        [System.Serializable] public class MenuPause {
            [Header("Configuration")]
            public TextMeshProUGUI textName;
            public TextMeshProUGUI textTime;

            public void ShowData(string name, float time) {  
                textName.SetText(name);
                textTime.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt(time / 60), Mathf.FloorToInt(time % 60),  Mathf.FloorToInt((time * 1000000) % 100));
            }
        }
        [System.Serializable] public class MenuDeath {
            [Header("Configuration")]
            public TextMeshProUGUI textName;
            public TextMeshProUGUI textTime;
            public TextMeshProUGUI textLevel;

            public void ShowData(string name, float time, string level) { 
                textName.SetText(name);
                textTime.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt(time / 60), Mathf.FloorToInt(time % 60),  Mathf.FloorToInt((time * 1000000) % 100));
                textLevel.SetText(level);
            }
        }   
        [System.Serializable] public class MenuCutscene {
            [Header("Configuration")]
            public TextMeshProUGUI textName;
            public TextMeshProUGUI textDesctiption;
            public Image image;
            public float timeChar = 0.05f;

            public void ShowData(DataCutscene.Data.Diapositive diapositive) { 
                textName.SetText(diapositive.nameDiapositive);
                ShowTextDialogue(diapositive.text);
                image.sprite = diapositive.illustration;
            }

            private void ShowTextDialogue(string dialogue) { 
                if(textCoroutine!=null) { CanvaManager.main.StopCoroutine(textCoroutine); }
                textCoroutine = CanvaManager.main.StartCoroutine(WriteText(dialogue));
            } 

            private Coroutine textCoroutine;
            IEnumerator WriteText(string text) { 
                string textWritter = ""; textDesctiption.SetText("");
                foreach (char c in text) {
                    textWritter += c; textDesctiption.SetText(textWritter);
                    yield return new WaitForSecondsRealtime(timeChar);
                }
            }
        }   
    }
}
