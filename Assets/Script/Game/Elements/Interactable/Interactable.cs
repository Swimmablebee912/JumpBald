using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : Element
{   
    [Header("Configuration")]
    public TypeInteractable type; 
    public State systemState;
    public Contacts systemContacts;

    public override void Initial(){ systemContacts.Initial(this); systemState.Initial(this); }
    public override void Update_Fixed() { if(systemState.inActive) systemContacts.CheckFixed(); }
    public override void Update_Check() { if(systemState.inActive) systemContacts.Check(); }
    public override void Update_Apply() { if(systemState.inActive) systemContacts.Apply(); }

    public virtual void State_StartContact(){  }
    public virtual void State_ExitContact(){  }
    public virtual void Action_Interact(){ systemState.CheckState(); }

    public virtual void Check_StateInteractable() { systemState.CheckState(); }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(systemContacts.collisionInteract.pivotCollision.position + systemContacts.collisionInteract.offset, systemContacts.collisionInteract.radius);
    }

    [System.Serializable] public class SystemElement_Interactable : SystemElement {
        [HideInInspector] public Interactable scrInteractable;
        public override void Initial(Element scr) { base.Initial(scr); scrInteractable = scr as Interactable;}
    }
    [System.Serializable] public class Contacts : SystemElement_Interactable {

        [Header("Reference")]
        public CheckCollision collisionInteract;
        public Animator anim;

        [Header("Read")]
        public bool inContact;

        public override void Check () { 
            if(inContact != collisionInteract.Check()) { inContact = collisionInteract.Check();
                if(inContact) StartContact(); else ExitContact();
            }
        }
        
        private void StartContact() { 
            Player.main.System_actions.Set_Interaction(scrInteractable); scrInteractable.State_StartContact(); 
            if(anim!=null) anim.SetBool("Contact", true);
        }
        public void ExitContact() { 
            Player.main.System_actions.Set_Interaction(null); scrInteractable.State_ExitContact(); 
            if(anim!=null) anim.SetBool("Contact", false);
        }
    }
    [System.Serializable] public class State : SystemElement_Interactable {
        [Header("Reference")]
        public GameObject root;
        public Animator anim;


        [Header("Configuration")]
        public bool initialActive = true;
        public List<DataAchievement> achievementToActive;
        public List<DataAchievement> achievementToInactive;

        [Header("Read")]
        public bool inActive;

        public override void Initial(Element scr)  { base.Initial(scr); CheckState(); }

        public void CheckState(){
            inActive = initialActive;
            
            if(inActive == false) CheckActive();
            if(inActive == true) CheckInactive();

            root.gameObject.SetActive(inActive);
            anim.enabled = inActive;
        }

        private void CheckActive() {
            inActive = true; foreach(DataAchievement data in achievementToActive) { if(!Player.main.System_state.Get_Achievement(data)) { inActive = false; break; } }
        }
        private void CheckInactive() { 
            foreach(DataAchievement data in achievementToInactive) { if(Player.main.System_state.Get_Achievement(data)) { inActive = false; break;  } }
        }
    }
}

public enum TypeInteractable { Checkpoint, NPC, Item }
