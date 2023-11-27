using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Item : Interactable
{
    public Interact interaction;

    public override void Initial(){  base.Initial();  interaction.Initial(this); }
    public override void Update_Fixed() { base.Update_Fixed(); }
    public override void Update_Check() { base.Update_Check(); interaction.Check(); }
    public override void Update_Apply() { base.Update_Apply(); interaction.Apply(); }

    public override void State_StartContact(){ LevelManager.main.systemInteract.systemItem.Set_ItemInteract(this); }
    public override void State_ExitContact(){ LevelManager.main.systemInteract.systemItem.Set_ItemInteract(null); }
    public override void Action_Interact() { interaction.Action_Interact(); base.Action_Interact(); State_ExitContact(); }

    [System.Serializable] public class SystemElement_InteractableItem : SystemElement_Interactable {
        [HideInInspector] public Interactable_Item scrItem;
        public override void Initial(Element scr) { base.Initial(scr); scrItem = scr as Interactable_Item;}
    }

    [System.Serializable] public class Interact : SystemElement_InteractableItem{
        [Header("Reference")]
        public Animator anim;

        [Header("State")]
        public TypeItem typeItem; public enum TypeItem { Item, Achievement, Weapon }
        public DataItem item;
        public DataAchievement achievement;
        public DataWeapon weapon;

        public virtual void Action_Interact() {
            if(item!=null) {
                switch(item.configuration.type) {
                    case DataItem.Configuration.TypeItem.Life: Player.main.System_state.ModifyHealth(item.configuration.count); break;
                }
            }
            if(achievement!=null) { Player.main.System_state.Add_Achievement(achievement); }
            if(weapon!=null) {  Player.main.System_state.Add_Weapon(weapon); }

            if(anim!=null) anim.SetTrigger("Interact");
        }
    }
}
