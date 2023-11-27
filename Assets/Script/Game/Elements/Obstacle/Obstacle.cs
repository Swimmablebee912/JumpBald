using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Obstacle : Element {

    [Header("Configuration")]
    public bool noUse_InStun;
    public bool reset_InIntro;

    [Header("Read")]
    public bool isActivated;
    
    public Contacts systemContacts;
    public ActionExternal systemExternal;
    public ActionSelf systemSelf;

    public virtual void Reset_Initial() { 
        if(reset_InIntro) { systemContacts.Reset_Initial(); systemExternal.Reset_Initial(); systemSelf.Reset_Initial();   }
    }
    
    public override void Initial(){ systemContacts.Initial(this); systemExternal.Initial(this); systemSelf.Initial(this); }
    
    public override void Update_Fixed() { 
        if(noUse_InStun) { 
            if(Player.main.System_gravity.dataRead.inStun) { 
                systemSelf.collision.collider2D.enabled = false;
                return; 
            } else { 
                systemSelf.collision.collider2D.enabled = !systemSelf.collision.isTrigger;
            }
        }
        systemContacts.CheckFixed(); systemExternal.CheckFixed(); systemSelf.CheckFixed(); 
    }
    public override void Update_Check() { 
        if(noUse_InStun) { if(Player.main.System_gravity.dataRead.inStun) { return; } }
        systemContacts.Check(); systemExternal.Check(); systemSelf.Check();  
    }
    public override void Update_Apply() { 
        if(noUse_InStun) { if(Player.main.System_gravity.dataRead.inStun) { return; } }
        systemContacts.Apply(); systemExternal.Apply(); systemSelf.Apply();  
    }

    public virtual void StartContact() { 
        if(noUse_InStun) { if(Player.main.System_gravity.dataRead.inStun) { return; } } 
        systemExternal.StartContact(); systemSelf.StartContact(); 
    }
    public virtual void StayContact() { 
        if(noUse_InStun) { if(Player.main.System_gravity.dataRead.inStun) { return; } } 
        systemExternal.StayContact(); systemSelf.StayContact(); 
    }
    public virtual void ExitContact() { 
        if(noUse_InStun) { if(Player.main.System_gravity.dataRead.inStun) { return; } } 
        systemExternal.ExitContact(); systemSelf.ExitContact(); 
    }

    public virtual void State_Activated(bool activated) { 
        isActivated = activated;
        if(noUse_InStun) { if(Player.main.System_gravity.dataRead.inStun) { return; } } 
        systemExternal.State_Activated(activated); systemSelf.State_Activated(activated);  
    }
    
    private void OnDrawGizmos() { 
    }
    private void OnDrawGizmosSelected() { 
        systemContacts.OnDrawGizmos(); 
        systemExternal.OnDrawGizmos(); 
        systemSelf.OnDrawGizmos(); 
        systemSelf.OnDrawGizmosSelected(); 
    }

    [System.Serializable] public class SystemElement_Obstacle : SystemElement { 
        [HideInInspector] public Obstacle scrObstacle;
        public bool isActivated = false;

        public virtual void Reset_Initial() {  }

        public override void Initial(Element scr) { base.Initial(scr); scrObstacle = scr as Obstacle;}

        public virtual void StartContact() {  }
        public virtual void StayContact() {  }
        public virtual void ExitContact() {  }

        public virtual void State_Activated(bool activated) { isActivated = activated; }
    }
    
    [System.Serializable] public class Contacts : SystemElement_Obstacle {
        [Header("Reference")]
        public CheckCollision_Cube collisionInteract;
        public SpriteRenderer spr;
        public Transform pivotSpr;

        [Header("Configuration")]
        public Vector2 offsetSprite = new Vector2(0.32f, 0.32f);
        public float shakeDuration, shakeIntensity;

        [Header("Read")]
        public bool inContact;

        public void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Matrix4x4 matrizOriginal = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(collisionInteract.pivotCollision.position + collisionInteract.offset, Quaternion.Euler(0, 0, collisionInteract.angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, collisionInteract.size * Vector2.one * 0.32f);
            Gizmos.matrix = matrizOriginal;
            Visual_SizeObstacle();
        }

        public override void Check () { 
            if(inContact != collisionInteract.Check()) { inContact = collisionInteract.Check(); if(inContact) StartContact(); else ExitContact(); }
            else if(inContact) StayContact();
        }
        
        private void Visual_SizeObstacle() { 
            spr.size = collisionInteract.size * Vector2.one * 0.32f - offsetSprite;
            spr.transform.rotation = Quaternion.Euler(0, 0, collisionInteract.angle);
        }    

        public void ShakeObject() { Vector2 positionSave = pivotSpr.localPosition;  pivotSpr.DOShakePosition(shakeDuration, shakeIntensity).OnComplete(()=>{ pivotSpr.localPosition = positionSave; });  }


        public override void StartContact() { scrObstacle.StartContact(); ShakeObject(); scrObstacle.State_Activated(false); }
        public override void StayContact() { scrObstacle.StayContact(); }
        public override void ExitContact() { scrObstacle.ExitContact(); }
    }
    
    [System.Serializable] public class ActionExternal : SystemElement_Obstacle {
        
        [Header("Reference")]
        public SpriteRenderer sprType;
        public Sprite sprBase, sprBaseEffector, sprDamage, sprBounce;

        [Header("Configuration")]
        public bool copyContact = true;
        public CheckCollision_Cube collisionInteract;

        [Header("Read")]
        public bool inContact;

        [Header("Test")]
        public Obstacle test_scrObstacle;

        public ActionDamage damage;
        public ActionBounce bounce;


        public void OnDrawGizmos() {
#if UNITY_EDITOR
            if(copyContact) { collisionInteract = new CheckCollision_Cube (test_scrObstacle.systemContacts.collisionInteract);  }           

            Gizmos.color = Color.red;
            Matrix4x4 matrizOriginal = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(collisionInteract.pivotCollision.position + collisionInteract.offset, Quaternion.Euler(0, 0, collisionInteract.angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, collisionInteract.size * Vector2.one * 0.32f);
            Gizmos.matrix = matrizOriginal;

            bounce.OnDrawGizmos();
            damage.OnDrawGizmos();

            if(damage.canDamage) { sprType.sprite = sprDamage; }
            else if(bounce.canBounce) { sprType.sprite = sprBounce; }
            else if(test_scrObstacle.systemSelf.collision.isEffectorPlattform) { sprType.sprite = sprBaseEffector; }
            else sprType.sprite = sprBase; 
#endif
        }

        public override void Reset_Initial() { damage.Reset_Initial(); bounce.Reset_Initial();  }

        public override void Initial(Element scr) { base.Initial(scr);
            damage.Initial(scr);
            bounce.Initial(scr);
        }

        public override void Apply() { base.Apply();

        }
        public override void Check() { base.Check();
            if(!copyContact) {
                if(inContact != collisionInteract.Check()) { inContact = collisionInteract.Check(); if(inContact) StartContact_External(); else ExitContact_External(); }
                else if(inContact) StayContact_External();
            } 
        }

        public override void StartContact() { if(!copyContact) return;  damage.StartContact(); bounce.StartContact(); }
        public override void StayContact() {  if(!copyContact) return; damage.StayContact(); bounce.StayContact(); }
        public override void ExitContact() {  if(!copyContact) return; damage.ExitContact(); bounce.ExitContact(); }
        public virtual void StartContact_External() { bounce.StartContact(); damage.StartContact();  }
        public virtual void StayContact_External() { bounce.StayContact(); damage.StayContact();  }
        public virtual void ExitContact_External() { bounce.ExitContact(); damage.ExitContact();  }

        public override void State_Activated(bool activated) { base.State_Activated(activated); damage.State_Activated(activated); bounce.State_Activated(activated); }
        
        [System.Serializable] public class ActionDamage : SystemElement_Obstacle {

            [Header("Configuration")]
            public bool canDamage = false; 
            public float timeToDamage = 2;
            public bool applyStay = true;

            [Header("Read")]
            public float time;
            
            public void OnDrawGizmos() {  }

            public override void Reset_Initial() {  }

            public override void StartContact() { 
                if(canDamage) { 
                    time=0;
                    Player.main.System_actions.Action_Hit(); 
                } 
            }
            public override void StayContact() { if(!applyStay) return;
                if(canDamage) { 
                    time+=Time.deltaTime;
                    if(time>=timeToDamage) {
                        Player.main.System_actions.Action_Hit(); 
                        time=0;
                    }
                } 
            }
            public override void ExitContact() { 
                if(canDamage) { 
                    time=0;
                } 
            }
        }
        [System.Serializable] public class ActionBounce : SystemElement_Obstacle {
            [Header("Reference")]
            public Transform root;

            [Header("Configuration")]
            public bool canBounce = false;
            public bool isRotated = false;
            public Vector2 directionInitial = Vector2.zero;
            public float forceBounce = 1;
            public float speedAngleRotated = 1;
            public float timeToBounce = .5f;
            public bool applyStay = true;


            [Header("Read")]
            public float angleBounce;
            public float time;
            

            public void OnDrawGizmos() {
                root.gameObject.SetActive(canBounce && (directionInitial!=Vector2.zero || isRotated) );
            }
            

            public override void Reset_Initial() {  angleBounce = 0; }

            public override void Initial(Element scr)  { 
                base.Initial(scr); 
                root.gameObject.SetActive(canBounce);
                angleBounce = GetAngle(directionInitial); 
            }
            public override void Check() { base.Check();
                if(canBounce){ if(isRotated) { RotateAngle(); } }
            }

            private void RotateAngle() {
                angleBounce += Time.deltaTime * speedAngleRotated;
                root.localRotation = Quaternion.Euler(0, 0, angleBounce);
            }

            private float GetAngle(Vector2 direction) {  return Mathf.Atan2(direction.y, direction.x); }
            private Vector2 GetDirection(float angleInRadians) {  return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)); }

            public override void StartContact() { 
                if(canBounce) { 
                    time = 0; 
                    Player.main.System_gravity.Action_Bounce(isRotated ? GetDirection(angleBounce) : directionInitial , forceBounce);
                    } 
                }
            public override void StayContact() { if(!applyStay) return;
                if(canBounce) { 
                    time+=Time.deltaTime;
                    if(time>=timeToBounce) {
                        Player.main.System_gravity.Action_Bounce(isRotated ? GetDirection(angleBounce) : directionInitial , forceBounce);
                        time=0;
                    }
                } 
            }
            public override void ExitContact() {  }

        }
    }
    [System.Serializable] public class ActionSelf : SystemElement_Obstacle {
        public ActionSelf_State state;
        public ActionCollision collision;
        public ActionSelf_Movement movement;

        public void OnDrawGizmos() { collision.OnDrawGizmos(); }
        public void OnDrawGizmosSelected() { state.OnDrawGizmosSelected();  movement.OnDrawGizmosSelected(); }

        public override void Reset_Initial() { state.Reset_Initial(); collision.Reset_Initial();  movement.Reset_Initial();  }

        public override void Initial(Element scr) { base.Initial(scr); 
            state.Initial(scr); 
            movement.Initial(scr); 
            collision.Initial(scr);
        }
        public override void CheckFixed()  { base.CheckFixed();  collision.CheckFixed();  }
        public override void Check() { base.Check(); state.Check(); movement.Check(); collision.Check();  }
        public override void Apply() { base.Apply(); state.Apply(); movement.Apply(); collision.Apply(); }

        public override void StartContact() { state.StartContact(); movement.StartContact(); collision.StartContact();}
        public override void StayContact() { state.StayContact(); movement.StayContact(); collision.StayContact(); }
        public override void ExitContact() { state.ExitContact(); movement.ExitContact(); collision.ExitContact(); }

        public override void State_Activated(bool activated) {  base.State_Activated(activated); state.State_Activated(activated); movement.State_Activated(activated); collision.State_Activated(activated);   }


        [System.Serializable] public class ActionCollision : SystemElement_Obstacle {
            [Header("Reference")]
            public Transform rootCollision;
            public BoxCollider2D collider2D;
            public PlatformEffector2D effector2D;

            [Header("Configuration")]
            public bool isTrigger;
            public bool canGrabPlayer;
            public bool ifPlayerIsUp = true;
            public float forceGrab = 1;
            public bool isEffectorPlattform;
            public float angleEffector;

            [Header("Read")]
            public bool inContact;
            public Vector3 oldPosition;
            public Vector2 distanceToContact;

            [Header("Test")]
            public Obstacle scrObstacle_EditorMode;


            public void OnDrawGizmos() {
                collider2D.size = scrObstacle_EditorMode.systemContacts.collisionInteract.size * Vector2.one * 0.32f - new Vector2(1.10f, 1.10f);
                collider2D.isTrigger = isTrigger;
                collider2D.usedByEffector = isEffectorPlattform;
                effector2D.enabled = isEffectorPlattform;
                effector2D.rotationalOffset = angleEffector;
                collider2D.enabled = !isTrigger;
            }
            
            public override void Reset_Initial() {  }

            public override void Initial(Element scr) { base.Initial(scr);
                collider2D.size = scrObstacle.systemContacts.collisionInteract.size * Vector2.one * 0.32f - new Vector2(0.86f, 0.86f);
                collider2D.isTrigger = isTrigger;
                collider2D.usedByEffector = isEffectorPlattform;
                effector2D.enabled = isEffectorPlattform;
                effector2D.rotationalOffset = angleEffector;
                collider2D.enabled = !isTrigger;
            }

            public override void Apply()  { base.Apply();
                if(canGrabPlayer) GrabPlayer();
            }
            public virtual void GrabPlayer() { 
                if(inContact) {
                    if (ifPlayerIsUp) {
                        if(Vector2.Dot(distanceToContact.normalized, Vector2.down) >= 0) {
                            Vector3 speedMove = rootCollision.position - oldPosition;
                            Player.main.transform.position += speedMove * forceGrab;
                            oldPosition = rootCollision.position;
                        }
                    } else {
                        Vector3 speedMove = rootCollision.position - oldPosition;
                        Player.main.transform.position += speedMove * forceGrab;
                        oldPosition = rootCollision.position;
                    }                    
                }
            }

            public override void StartContact() { 
                inContact=true; 
                distanceToContact = Player.main.transform.position - collider2D.transform.position; 
                oldPosition = rootCollision.position;
            }
            public override void StayContact() {  }
            public override void ExitContact() { inContact=false; distanceToContact = Vector2.zero; }

        }
        [System.Serializable] public class ActionSelf_State : SystemElement_Obstacle {
            [Header("Reference")]
            public CheckCollision_Cube checkCollision;

            [Header("Configuration")]
            public TypeActivate activeIn; public enum TypeActivate { Allways, Start, Stay, Exit } 
            public float timeEnabled = 1;
            public float timeActivated = 0;
            

            [Header("Read")]
            public bool isEnabled = false;
            public bool inContact;

            public void OnDrawGizmosSelected() {
                Gizmos.color = Color.yellow;
                Matrix4x4 matrizOriginal = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(checkCollision.pivotCollision.position + checkCollision.offset, Quaternion.Euler(0, 0, checkCollision.angle), Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, checkCollision.size * Vector2.one * 0.32f);
                Gizmos.matrix = matrizOriginal;
                
            }

            public override void Reset_Initial() {  }

            public override void Initial(Element scr) { base.Initial(scr);
                scrObstacle.StartCoroutine(StartEnabled());
            }
            public override void Check() { base.Check();
                if(!isEnabled) return;

                if(activeIn == TypeActivate.Allways) { isActivated = true; }
                else {
                    if(inContact != checkCollision.Check()) { 
                        inContact = checkCollision.Check(); 
                        if(inContact) { if(activeIn == TypeActivate.Start) { if(coroutineActivated==null) coroutineActivated = scrObstacle.StartCoroutine(StartActivated());}  } 
                        else { if(activeIn == TypeActivate.Exit) { if(coroutineActivated==null) coroutineActivated = scrObstacle.StartCoroutine(StartActivated()); } }
                    }
                    else { 
                        if(inContact) { if(activeIn == TypeActivate.Stay) { isActivated = true; }  } 
                        else { if(activeIn == TypeActivate.Stay) { isActivated = false; }  }
                    }
                }

                if (scrObstacle.isActivated != isActivated) scrObstacle.State_Activated(isActivated);
            }

            public override void State_Activated(bool activated) {  base.State_Activated(activated);
                if(activeIn!=TypeActivate.Allways) {
                    if(activated==false) { 
                        isEnabled = false; 
                        scrObstacle.StartCoroutine(StartEnabled()); 
                    } 
                }
            }

            private IEnumerator StartEnabled() { yield return new WaitForSeconds(timeEnabled); isEnabled = true; }
            Coroutine coroutineActivated; private IEnumerator StartActivated() { yield return new WaitForSeconds(timeActivated); isActivated = true; coroutineActivated= null; }
        }
        [System.Serializable] public class ActionSelf_Movement : SystemElement_Obstacle {
            [Header("Reference")]
            public Transform pivotCentral;
            public Transform pivotMove;

            [Header("Configuration")]
            public float speedMove;
            public List<Vector2> targetsPositions;
            public bool loop;
            public bool toFirstPos_inDesactive;

            [Header("Read")]
            public int idPos;
            public Vector3 positionInitial;
            public List<Vector3> worldTargetsPositions;

            public void OnDrawGizmosSelected() {
                Gizmos.color = Color.red;
#if UNITY_EDITOR
                positionInitial = pivotCentral.position;
#endif
                foreach(Vector2 targetPos in targetsPositions){ Gizmos.DrawSphere(positionInitial + (Vector3)targetPos, .1f); }
            }

            public override void Reset_Initial() { ResetPlattform(); }

            public override void Initial(Element scr) { base.Initial(scr);
                positionInitial = pivotCentral.position;
                worldTargetsPositions =  new List<Vector3>(); foreach(Vector2 targetPos in targetsPositions){ worldTargetsPositions.Add(positionInitial + (Vector3)targetPos); }
            }
            public override void Check() { base.Check();
                if(worldTargetsPositions!=null) { if(worldTargetsPositions.Count>0) MovePlattform(); }
            }

            public void MovePlattform() {
                if(scrObstacle.isActivated) {
                    if (Vector3.Distance(pivotMove.position, worldTargetsPositions[idPos]) < speedMove * Time.deltaTime) { ChangePlattform(); } 
                    else { pivotMove.position = pivotMove.position + (worldTargetsPositions[idPos] - pivotMove.position).normalized * speedMove * Time.deltaTime; }
                } 
                else {
                    if(toFirstPos_inDesactive) { 
                        if (Vector3.Distance(pivotMove.position, worldTargetsPositions[0]) > speedMove/2f * Time.deltaTime) { 
                            pivotMove.position = pivotMove.position + (worldTargetsPositions[0] - pivotMove.position).normalized * speedMove/2f * Time.deltaTime; 
                        }
                    }
                }
            }
            public void ResetPlattform() { Initial(scrElement); idPos=0; pivotMove.position = worldTargetsPositions[idPos]; }
            public void ChangePlattform() {
                idPos++; 
                if(idPos >= worldTargetsPositions.Count) { 
                    scrObstacle.State_Activated(false);
                    if(loop) { idPos = 0; } else { idPos = worldTargetsPositions.Count-1; }
                }
            }

            public override void StartContact() { 
            }
            public override void StayContact() { 
            }
            public override void ExitContact() { 
                
            }
        }
    }
}
