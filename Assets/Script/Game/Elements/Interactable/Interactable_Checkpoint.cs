using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Checkpoint : Interactable
{
    public Interact interaction;

    public override void Initial(){ 
        base.Initial(); interaction.Initial(this);
    }
    public override void Update_Fixed() { base.Update_Fixed(); }
    public override void Update_Check() { base.Update_Check(); interaction.Check(); }
    public override void Update_Apply() { base.Update_Apply(); interaction.Apply(); }

    public override void State_StartContact(){ 
        LevelManager.main.systemInteract.systemCheckpoint.Set_CheckpointInteract(this); 
        LevelManager.main.systemTimer.PauseTimer(true);
    }
    public override void State_ExitContact(){ 
        LevelManager.main.systemInteract.systemCheckpoint.Set_CheckpointInteract(null); 
        LevelManager.main.systemTimer.PauseTimer(false);
    }
    public override void Action_Interact() { interaction.Action_Interact();  }

    public override void Check_StateInteractable() { interaction.CheckState(); }

    [System.Serializable] public class SystemElement_InteractableCheckpoint : SystemElement_Interactable {
        [HideInInspector] public Interactable_Checkpoint scrCheckpoint;
        public override void Initial(Element scr) { base.Initial(scr); scrCheckpoint = scr as Interactable_Checkpoint;}
    }

    [System.Serializable] public class Interact : SystemElement_InteractableCheckpoint{
        [Header("Reference")]
        public Animator anim;
        public DataCheckpoint data;

        [Header("Configuration")]
        public DataAchievement achievementToAdd;

        [Header("Read")]
        public bool isActive;
        public bool isCurrent;

        public override void Initial(Element scr)  { base.Initial(scr); CheckState(); CheckCurrent(); }

        public virtual void Action_Interact() {
            CheckCurrent();
            Player.main.System_state.RestoreHealth();
            if(anim!=null) { anim.SetTrigger("Interact"); }
            if(isCurrent) { 
                if(Player.main.System_state.dataRead.health<Player.main.System_state.dataConfiguration.healthMax) { 
                    CanvaManager.main.notification.ShowNotification("Rest Bonfire", 2);
                } else {
                    if (Player.main.System_state.ChangeCheckpoint(1)) CanvaManager.main.notification.ShowNotification("Travel Bonfire", 2);
                    else { CanvaManager.main.notification.ShowNotification("Rest Bonfire", 2); }
                }
            } 
            else {
                isCurrent = true;
                if (!isActive) Player.main.System_state.Add_Achievement(achievementToAdd);
                Player.main.System_state.SetCheckpoint( data );
                CanvaManager.main.notification.ShowNotification("Rest Bonfire", 2);
                CheckState(); CheckCurrent();
                LevelManager.main.systemInteract.systemCheckpoint.Set_CheckpointInteract(scrCheckpoint);
            }
        }

        public void CheckCurrent() {
            isCurrent = Player.main.System_state.is_CheckpointCurrent(data);
        }
        public void CheckState(){ 
            isActive = Player.main.System_state.is_Checkpoint(data);
            if(anim!=null) { anim.SetBool("Active", isActive ); }
        }
    }
}