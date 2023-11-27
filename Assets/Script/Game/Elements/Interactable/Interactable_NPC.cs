using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_NPC : Interactable
{
    public Interact interaction;

    public override void Initial(){ base.Initial(); interaction.Initial(this); }
    public override void Update_Fixed() { base.Update_Fixed(); }
    public override void Update_Check(){ base.Update_Check(); interaction.Check();  }
    public override void Update_Apply(){ base.Update_Apply(); interaction.Apply();  }

    public override void State_StartContact(){ interaction.Action_ShowDialogue_Intro();  }
    public override void State_ExitContact(){ interaction.Action_ShowDialogue_Exit(); }
    public override void Action_Interact() { interaction.Action_Interact(); base.Action_Interact(); }
    
    [System.Serializable] public class SystemElement_InteractableNPC : SystemElement_Interactable {
        [HideInInspector] public Interactable_NPC scrNPC;
        public override void Initial(Element scr) { base.Initial(scr); scrNPC = scr as Interactable_NPC;}
    }
    
    [System.Serializable] public class Interact : SystemElement_InteractableNPC {
        [Header("Reference")]
        public Animator anim;

        [Header("Configuration")]
        public DataNPC data;
        public DataCutscene toCutscene;

        [Header("Read")]
        public int countDialogue=0;

        public void Action_Interact() { Action_ShowDialogue_Next(); if(anim!=null) anim.SetTrigger("Interact"); }

        public void Action_ShowDialogue_Intro(){ countDialogue=0; LevelManager.main.systemInteract.systemDialogue.Set_Dialogue(scrElement as Interactable_NPC, data.configuration.dialogues[countDialogue]); }
        public void Action_ShowDialogue_Next() { 
            countDialogue = Math.Clamp(countDialogue+1, 0 , data.configuration.dialogues.Count); 
            if(countDialogue >= data.configuration.dialogues.Count) Action_ShowDialogue_Exit(); 
            else LevelManager.main.systemInteract.systemDialogue.Set_Dialogue(scrElement as Interactable_NPC, data.configuration.dialogues[countDialogue]); 
        }
        public void Action_ShowDialogue_Exit(){ 
            countDialogue = -1; LevelManager.main.systemInteract.systemDialogue.Set_Dialogue(null, null); 
            if(toCutscene!=null) { 
                LevelManager.main.Function_Button_CutsceneStart(toCutscene);
            }
        }
        
    }
}
