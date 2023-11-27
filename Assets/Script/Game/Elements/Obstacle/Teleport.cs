using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : Element {
    public bool isActivated;
    public Interacts systemInteracts;

    public override void Initial(){ systemInteracts.Initial(this);  }
    public override void Update_Fixed() { systemInteracts.CheckFixed(); }
    public override void Update_Check() { systemInteracts.Check();  }
    public override void Update_Apply() { systemInteracts.Apply();   }

    public virtual void StartContact(bool idContact) {  }

    public virtual void State_Activated(bool activated) { isActivated = activated; }

    private void OnDrawGizmos() { systemInteracts.OnDrawGizmos(); }

    [System.Serializable] public class SystemElement_Obstacle : SystemElement { 
        [HideInInspector] public Obstacle scrObstacle;
        public bool isActivated = false;

        public override void Initial(Element scr) { base.Initial(scr); scrObstacle = scr as Obstacle;}

        public virtual void StartContact() {  }
        public virtual void StayContact() {  }
        public virtual void ExitContact() {  }

        public virtual void State_Activated(bool activated) { isActivated = activated; }
    }
    
    [System.Serializable] public class Interacts : SystemElement_Obstacle {
        [Header("Configuration")]
        public List<Doors> doors;

        public void OnDrawGizmos() { Gizmos.color = Color.yellow; foreach(Doors door in doors) { door.OnDrawGizmos(); }  }
        public override void Check () { 
            foreach(Doors door in doors) { 
                if(door.Check()) if(door.CheckContact(Player.main.System_gravity.dataRead.directionVelocity)) Activate(door, doors[door.outId]); 
            } 
        }
        
        public void Activate(Doors doorToActivate, Doors doorToMove) { 
            Player.main.transform.position = doorToMove.pivotToMove.position;
            Player.main.System_gravity.Set_StateTurn(true);
            if(doorToMove.speedToApply!=Vector2.zero) Player.main.System_gravity.Set_Velocity(doorToMove.speedToApply);
            if (doorToActivate.anim!=null) doorToActivate.anim.SetTrigger("Open"); 
            if (doorToMove.anim!=null) doorToMove.anim.SetTrigger("Close");
        }

        [System.Serializable] public class Doors {
            public string nameReference;

            [Header("Reference")]
            public CheckCollision collisionInteract;
            public Animator anim;

            [Header("Configuration")]
            public Vector2 directionCheck;
            public Transform pivotToMove;
            public int outId;
            public Vector2 speedToApply;

            [Header("Read")]
            public bool inContact;

            public void OnDrawGizmos() { Gizmos.DrawWireSphere(collisionInteract.pivotCollision.position + collisionInteract.offset, collisionInteract.radius); }
            public bool Check() { if(inContact != collisionInteract.Check()) { inContact = collisionInteract.Check(); } return inContact; }

            public bool CheckContact( Vector2 directionToCheck ) { return inContact && (Vector2.Dot(directionCheck.normalized, directionToCheck.normalized) >= 0.5f ); }
        }
    }
    
}
