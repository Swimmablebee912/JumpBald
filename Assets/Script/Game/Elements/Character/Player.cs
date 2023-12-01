using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using DG.Tweening;


public class Player : Element
{
    public static Player main; private void Awake()  { if(main==null) { main=this;} else { Destroy(gameObject); } }
        
    [Header("Systems")]
    public State System_state;
    public Gravity System_gravity;
    public Actions System_actions;
    public Audio System_audio;

    public override void Initial(){ 
        System_state.Initial(this); System_gravity.Initial(this); System_actions.Initial(this); 
    }
    public override void Update_Fixed(){ if(!LevelManager.main.IsPlaying()) return;
        System_state.CheckFixed(); System_gravity.CheckFixed(); System_actions.CheckFixed();
    }
    public override void Update_Check(){ if(!LevelManager.main.IsPlaying()) return;
        System_state.Check(); System_gravity.Check(); System_actions.Check(); 
    }
    public override void Update_Apply(){ if(!LevelManager.main.IsPlaying()) return;
        System_state.Apply(); System_gravity.Apply(); System_actions.Apply(); 
    }

    private void OnDrawGizmos() {
        System_gravity.OnDrawGizmos();
    }

    public void LoadData() {
        System_state.dataRead.name = LevelManager.main.systemRun.dataRun.name; 
        System_state.dataRead.health = LevelManager.main.systemRun.dataRun.life; 
        System_state.dataConfiguration.healthMax = LevelManager.main.systemRun.dataRun.lifeMax;

        if(LevelManager.main.systemRun.dataRun.achievements!=null) { 
            System_state.dataRead.achievements  = new List<DataAchievement>{}; 
            foreach(string data in LevelManager.main.systemRun.dataRun.achievements){  System_state.dataRead.achievements.Add(LevelManager.main.systemData.GetAchievement(data)); }
        }
        if(LevelManager.main.systemRun.dataRun.weapons!=null) { 
            System_state.dataRead.weapons = new List<DataWeapon>{}; 
            foreach(string data in LevelManager.main.systemRun.dataRun.weapons){ System_state.dataRead.weapons.Add(LevelManager.main.systemData.GetWeapon(data)); }
        }
        if(LevelManager.main.systemRun.dataRun.checkpoints!=null) { 
            System_state.dataRead.checkpoints = new List<DataCheckpoint>{}; 
            foreach(string data in LevelManager.main.systemRun.dataRun.checkpoints){ System_state.dataRead.checkpoints.Add(LevelManager.main.systemData.GetCheckpoint(data)); }
        }

        System_gravity.dataRead.currentRoom = LevelManager.main.systemRun.dataRun.currentRoom;
        System_gravity.dataReference.pivot.position = LevelManager.main.systemRun.dataRun.positionWorld;
        LevelManager.main.systemTimer.time = LevelManager.main.systemRun.dataRun.time;
    }

    public void Function_Button_Interact(){ System_actions.Action_TapGround(); }

    [System.Serializable] public class SystemElement_Player : SystemElement {
        [HideInInspector] public Player scrPlayer;
        public override void Initial(Element scr) { base.Initial(scr); scrPlayer = scr as Player;}
    }
    [System.Serializable] public class Gravity : SystemElement_Player {
        public DataReference dataReference;
        public DataConfiguration dataConfiguration;
        public DataRead dataRead;

        public void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(dataRead.positionCenterToCheck, .1f);
            Gizmos.DrawWireSphere(dataRead.hit1.point, .125f); Gizmos.DrawWireSphere(dataRead.hit2.point, .125f);
            Gizmos.DrawWireSphere(dataReference.checkCollStage.pivotCollision.position + dataReference.checkCollStage.offset, dataReference.checkCollStage.radius);
            Gizmos.DrawWireSphere(dataRead.positionContact, .1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(dataReference.checkBody.pivotCollision.position + dataReference.checkBody.offset, dataReference.checkBody.radius);
            
            Vector3 centro = dataReference.checkCollStage.pivotCollision.position + dataReference.checkCollStage.offset;
            float radio = dataReference.checkCollStage.radius;
            Vector3 comprobacion = dataRead.directionContact.normalized;
            Vector3 direccion = dataRead.directionCheckColl.normalized;

            float similitud = Vector3.Dot(direccion.normalized, comprobacion.normalized);

            Gizmos.color = Color.blue; Gizmos.DrawLine(centro, centro + direccion.normalized * radio);

            if (similitud > dataConfiguration.vectorSimilitud_DetectedContact)   Gizmos.color = Color.green;  else Gizmos.color = Color.red;
            DrawCone(centro, comprobacion.normalized , dataConfiguration.vectorSimilitud_DetectedContact, radio);
        }
        private void DrawCone(Vector3 center, Vector3 direction, float similarityThreshold, float length)  {
            if(direction!=Vector3.zero) {
                float angle = Mathf.Acos(similarityThreshold) * Mathf.Rad2Deg * 2f;

                Quaternion coneRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                Vector2 basePoint1 = center + coneRotation * Quaternion.Euler(0, 0, angle / 2) * Vector2.right * length;
                Vector2 basePoint2 = center + coneRotation * Quaternion.Euler(0, 0, -angle / 2) * Vector2.right * length;

                Gizmos.DrawLine(center, basePoint1);
                Gizmos.DrawLine(center, basePoint2);
            }
        }

        public override void CheckFixed() { }
        public override void Check () { 
            Check_Collision();
            Check_Gravity();
            Check_Hit();
        }
        public override void Apply() { 
            Apply_Rotation(); 
            Apply_Animation(); 
        } 
  
        private void Check_Collision(){
            dataRead.positionCenterToCheck = dataReference.checkCollStage.pivotCollision.position + dataReference.checkCollStage.offset; // Get_PositionInGrid(dataReference.checkCollStage.pivotCollision.position + dataReference.checkCollStage.offset); //

            if (dataReference.checkCollStage.CheckEffector(dataReference.rigid.velocity.normalized)) {  
                dataRead.positionContact = dataReference.checkCollStage.GetClosePoint_Simple(); //Get_PositionInGrid(dataReference.checkCollStage.GetClosePoint_Simple()); } //
                dataRead.directionContact = (dataRead.positionContact - dataRead.positionCenterToCheck).normalized; 
                dataRead.collStage = dataReference.checkCollStage.coll;
                dataRead.directionVelocity = dataReference.rigid.velocity.normalized;
                if(dataRead.inStun){ 
                    if(dataRead.inAir && dataReference.rigid.velocity.normalized!=Vector2.zero) dataRead.directionCheckColl = dataReference.rigid.velocity.normalized; 
                    else { dataRead.directionCheckColl = (dataRead.positionContact - dataRead.positionCenterToCheck).normalized; }
                }
                else { dataRead.directionCheckColl = (dataRead.positionContact - dataRead.positionCenterToCheck).normalized; }
            } 
            else { 
                dataRead.positionContact = dataRead.positionCenterToCheck; 
                dataRead.directionContact = dataReference.rigid.velocity.normalized;
                dataRead.collStage = null;
                dataRead.directionVelocity = dataReference.rigid.velocity.normalized;
                dataRead.directionCheckColl =  dataReference.rigid.velocity.normalized;
            }

            if(dataReference.checkBody.CheckEffector(dataReference.rigid.velocity.normalized)) {
                dataRead.directionContact = (dataRead.positionContact - dataRead.positionCenterToCheck).normalized;
                if(dataRead.inStun) { 
                    dataRead.inAir = !Get_VectorIsSimilar(dataRead.directionContact.normalized, Vector2.down,  dataConfiguration.vectorSimilitud_DetectedContact);
                    if(coroutineContactNoGround!=null) { 
                        scrElement.StopCoroutine(coroutineContactNoGround); dataReference.textCount.gameObject.SetActive(false); 
                        coroutineContactNoGround=null;
                    } 
                }
                else {
                    if(Get_VectorIsSimilar(dataRead.directionContact.normalized, dataRead.directionCheckColl,  dataConfiguration.vectorSimilitud_DetectedContact) || dataReference.rigid.velocity.normalized == Vector2.zero) {
                        if(scrPlayer.System_state.GetWeapon().configuration.type == DataWeapon.Configuration.TypeAction.WallCling) {
                            if(dataRead.inTurn) {
                                dataRead.inAir = !Get_VectorIsSimilar(dataRead.directionContact.normalized, Vector2.down,  dataConfiguration.vectorSimilitud_DetectedContact); 
                                if(coroutineContactNoGround!=null) { 
                                    scrElement.StopCoroutine(coroutineContactNoGround); dataReference.textCount.gameObject.SetActive(false); 
                                    coroutineContactNoGround=null;
                                } 
                            } 
                            else { 
                                dataRead.inAir = false;  
                                if(!Get_VectorIsSimilar(dataRead.directionContact.normalized, Vector2.down,  dataConfiguration.vectorSimilitud_DetectedContact)) {
                                    if(coroutineContactNoGround==null) coroutineContactNoGround = scrElement.StartCoroutine(Coroutine_ContactNoGround()); 
                                } 
                                else { 
                                    if(coroutineContactNoGround!=null) { 
                                        scrElement.StopCoroutine(coroutineContactNoGround); dataReference.textCount.gameObject.SetActive(false); 
                                        coroutineContactNoGround=null;
                                    } 
                                }
                            }
                        } 
                        else {  dataRead.inAir = !Get_VectorIsSimilar(dataRead.directionContact.normalized, Vector2.down,  dataConfiguration.vectorSimilitud_DetectedContact);  }
                    } 
                    else { 
                        dataRead.inAir = true; 
                        if(coroutineContactNoGround!=null) { 
                            scrElement.StopCoroutine(coroutineContactNoGround); dataReference.textCount.gameObject.SetActive(false); 
                            coroutineContactNoGround=null;
                        } 
                    }
                }
            }
            else { 
                dataRead.inAir = true;
                if(coroutineContactNoGround!=null) { 
                    scrElement.StopCoroutine(coroutineContactNoGround); dataReference.textCount.gameObject.SetActive(false); 
                    coroutineContactNoGround=null;
                } 
            }

            float angulo = Vector2.SignedAngle(Vector2.right, dataRead.directionCheckColl);
            Vector3 direccionCheckFixed = Quaternion.Euler(0, 0, angulo) * new Vector3(dataConfiguration.directionToCheck.x, dataConfiguration.directionToCheck.y, 0);

            dataRead.hit1 = Physics2D.Raycast( dataRead.positionCenterToCheck + direccionCheckFixed, dataRead.directionCheckColl.normalized, Mathf.Infinity, dataReference.checkCollStage.groundLayer);
            dataRead.hit2 = Physics2D.Raycast( dataRead.positionCenterToCheck + -direccionCheckFixed, dataRead.directionCheckColl.normalized, Mathf.Infinity, dataReference.checkCollStage.groundLayer);
            dataRead.hitCentral = Physics2D.Raycast( dataRead.positionCenterToCheck - (Vector3)dataRead.directionCheckColl.normalized, dataRead.directionCheckColl.normalized, Mathf.Infinity, dataReference.checkCollStage.groundLayer);

            dataRead.angleCheckColl = Vector2.SignedAngle(Vector2.down, dataRead.directionContact);
            dataRead.distanceToMove = (dataRead.hitCentral.point - (Vector2)dataReference.checkBody.pivotCollision.position).magnitude;

            Debug.DrawRay( dataRead.positionCenterToCheck + direccionCheckFixed, dataRead.directionCheckColl.normalized, Color.red);
            Debug.DrawRay( dataRead.positionCenterToCheck + -direccionCheckFixed, dataRead.directionCheckColl.normalized, Color.red);
            Debug.DrawRay( dataRead.positionCenterToCheck - (Vector3)dataRead.directionCheckColl.normalized, dataRead.directionCheckColl.normalized * dataRead.distanceToMove, Color.red);
        }
        private void Check_Gravity() {
            if(!dataRead.inAir) {  
                if(!dataRead.inContact) { dataRead.inContact=true; Action_Contact(); }
                if(dataRead.inContact) {
                    if(!dataRead.inStun) { 
                        dataReference.rigid.gravityScale = 0; 
                        dataReference.rigid.velocity = Vector2.zero;    
                    } 
                    else { 
                        dataReference.rigid.gravityScale = 1; 
                        if (dataRead.angleCheckColl > -10 && dataRead.angleCheckColl < 10) { 
                            dataReference.rigid.velocity = new Vector2(dataReference.rigid.velocity.x - (dataReference.rigid.velocity.x * dataConfiguration.frictionGround * Time.deltaTime), dataReference.rigid.velocity.y); 
                            if(Math.Abs(dataReference.rigid.velocity.magnitude)<0.2f) dataReference.rigid.velocity = Vector2.zero;
                            if(dataReference.rigid.velocity == Vector2.zero) { if(coroutineStunExit==null) coroutineStunExit = scrElement.StartCoroutine(Coroutine_ExitStun()); } 
                        }
                        else { 
                            dataReference.rigid.velocity = new Vector2(Math.Clamp(-dataRead.angleCheckColl,-90,90)/90, -1) * dataConfiguration.gravity * dataConfiguration.gravityDown; 
                            if(dataRead.positionBody == dataReference.pivot.position) { dataReference.pivot.position += new Vector3( dataReference.rigid.velocity.x*0.25f, 0, 0); }
                            else { dataRead.positionBody = dataReference.pivot.position; }
                        }
                    }
                    if(!dataRead.inStun) dataRead.timeInFall=0; 
                }
            } 
            else { 
                dataReference.rigid.gravityScale = dataReference.rigid.velocity.y > 0 ? ( dataRead.inStun ? dataConfiguration.gravity * dataConfiguration.gravityStun : dataConfiguration.gravity * dataConfiguration.gravityUp) :  dataConfiguration.gravity * dataConfiguration.gravityDown; 
                if(dataReference.rigid.velocity.y<0) { dataRead.timeInFall += Time.deltaTime; } 
                dataRead.inContact = false;
                if(coroutineStunExit!=null) { scrElement.StopCoroutine(coroutineStunExit); coroutineStunExit=null; }
            }
            dataReference.rigid.velocity = new Vector2(Math.Clamp(dataReference.rigid.velocity.x ,-dataConfiguration.maxSpeed.x, dataConfiguration.maxSpeed.x), Math.Clamp(dataReference.rigid.velocity.y ,-dataConfiguration.maxSpeed.y, dataConfiguration.maxSpeed.y));
            
            if(!dataRead.inAir) { if(dataRead.inContact) { if(dataReference.rigid.velocity==Vector2.zero) { if(!dataRead.isSaved) { dataRead.isSaved = true; SaveData(); } }} }
            else { dataRead.isSaved = false; }
        }
        private void Check_Hit(){ 
            if(dataRead.inTurn && dataRead.inAir) {
                if(dataReference.checkBody.CheckEffector(dataReference.rigid.velocity.normalized)) { 
                    if(Get_VectorIsSimilar(dataRead.directionContact.normalized, dataRead.directionCheckColl, dataConfiguration.vectorSimilitud_DetectedHit)) { 
                        if(!Get_VectorIsSimilar(dataRead.directionContact.normalized, Vector2.down, dataConfiguration.vectorSimilitud_DetectedContact)) 
                            Action_Hit(dataRead.directionContact.normalized); 
                    } 
                } 
            }
        }

        private void Apply_Rotation(){ 
            dataReference.pivotRoot.localRotation = Quaternion.Euler(0, 0, dataRead.angleCheckColl);  // * (1 - Math.Clamp(dataRead.distanceToMove, 0, 1)) ); 
            float distanceClamped = Get_MapedClamp01(Math.Clamp(dataRead.distanceToMove, dataReference.checkBody.radius, dataReference.checkCollStage.radius), dataReference.checkBody.radius, dataReference.checkCollStage.radius );
            Vector3 pos = dataReference.pivot.InverseTransformPoint(dataRead.positionContact)  + ((Vector3)dataRead.directionContact.normalized) * dataConfiguration.distanceToFixedPosition;
            dataReference.pivotCenter.localPosition = pos * (1 - distanceClamped);
            Vector2 deformationScale = new Vector2(1 + Mathf.Max(dataReference.rigid.velocity.normalized.x, 0) * dataConfiguration.scaleDeformation, 1 + Mathf.Max(dataReference.rigid.velocity.normalized.y, 0) * dataConfiguration.scaleDeformation);
            dataReference.pivotCenter.localScale = new Vector3( deformationScale.x /  dataReference.pivotCenter.lossyScale.x, deformationScale.y /  dataReference.pivotCenter.lossyScale.y, 1.0f);
            if (dataRead.inAir && dataReference.rigid.velocity.x!=0) {
                dataRead.isMirror = dataReference.rigid.velocity.x<0;
                dataReference.pivotCenter.localScale = new Vector3( dataRead.isMirror? -1 : 1 , 1, 1);
            }
            
        } 
        private void Apply_Animation () {
            dataReference.anim.SetFloat("SpeedY", dataReference.rigid.velocity.y);
            dataReference.anim.SetBool("Air", dataRead.inAir);
            dataReference.anim.SetBool("Stun", dataRead.inStun);
            dataReference.anim.SetBool("Turn", dataRead.inTurn);
        }

        public void Set_Velocity(Vector2 newSpeed) { dataReference.rigid.velocity = newSpeed;  }
        public void Set_StateTurn(bool state) { if(dataRead.inTurn != state) { dataRead.inTurn = state; } }
        public void SetCurrentRoom(DataRoom newRoom) { dataRead.currentRoom = newRoom; LevelManager.main.systemRun.dataRun.currentRoom = newRoom; }

        private void Action_Contact() {
            if(dataRead.timeInFall > dataConfiguration.timeStunForFall) { 
                if (dataReference.rigid.velocity.y < 0) {  Action_Hit(Vector2.down); dataReference.rigid.velocity = new Vector2(dataReference.rigid.velocity.x, -5); } 
                EffectManager.main.InstanceEffect("Dust_ContactBig", dataRead.positionContact).SetDust("Normal").SetRotation(dataRead.angleCheckColl); 
                if (Get_Comprobation_ContactRoom()) { if(dataRead.inStun) { if(!dataRead.isHitted) { dataRead.isHitted = true; scrPlayer.System_state.ModifyHealth(-1); }} }
            }
            else { EffectManager.main.InstanceEffect("Dust_ContactLittle", dataRead.positionContact).SetDust("Normal").SetRotation(dataRead.angleCheckColl);  } 

            if(Get_VectorIsSimilar( dataRead.directionContact, Vector2.down, 0.25f)) { scrPlayer.System_audio.PlaySFX("Contact"); }

            CameraManager.main.AddShake("Contact");
            EffectManager.main.InstanceEffect("PopUp", dataReference.pivotRoot.position).SetPopUp("Contact", TypePopUp.action); 
            
            dataRead.contactRoom = dataRead.currentRoom;
        }
        public void Action_Hit(Vector2 directionHit) { 
            scrPlayer.System_audio.PlaySFX("Hit");
            dataReference.anim.SetTrigger("Hit");
            CameraManager.main.AddShake("Hit");
            EffectManager.main.InstanceEffect("PopUp", dataReference.pivot.position).SetPopUp("Hit", TypePopUp.action);
            EffectManager.main.InstanceEffect("Dust_Hit", dataRead.positionContact).SetDust("Normal").SetRotation(dataRead.angleCheckColl); 
            EffectManager.main.InstanceEffect("Dust_ContactLittle", dataRead.positionContact).SetDust("Normal").SetRotation(dataRead.angleCheckColl); 
            if(!dataRead.inStun) { dataRead.inStun=true; }
            if(Get_VectorIsSimilar(directionHit, Vector2.up, .9f)) { dataReference.rigid.velocity = new Vector2( dataReference.rigid.velocity.x * .45f,  Math.Clamp(dataReference.rigid.velocity.y * .25f, -5 , 5));  } 
            else if(Get_VectorIsSimilar(directionHit, Vector2.down, .9f)) { dataReference.rigid.velocity = new Vector2( dataReference.rigid.velocity.x * .25f, -Math.Clamp(dataReference.rigid.velocity.y * .25f, -5 , 5));  } 
            else { dataReference.rigid.velocity = new Vector2( -dataReference.rigid.velocity.x * .5f, Math.Clamp(dataReference.rigid.velocity.y * .75f, -5 , 5));   } 
        }

        public void Action_Bounce(Vector2 directionBounce, float forceBounce) { 
            if(directionBounce!=Vector2.zero) { dataReference.rigid.velocity = directionBounce.normalized * forceBounce; } 
            else {  dataReference.rigid.velocity = -dataReference.rigid.velocity.normalized * forceBounce; }
        }

        private void SaveData() { 
            LevelManager.main.systemRun.dataRun.currentRoom = dataRead.currentRoom;
            LevelManager.main.systemRun.dataRun.positionWorld = dataReference.pivot.position;
        }

        Coroutine coroutineStunExit; IEnumerator Coroutine_ExitStun(){ 
            yield return new WaitForSeconds(.5f); 
            dataRead.inStun=false; 
            EffectManager.main.InstanceEffect("PopUp", dataReference.pivotRoot.position).SetPopUp("Up", TypePopUp.action); 
            if(dataRead.isHitted) dataRead.isHitted = false; 
            coroutineStunExit=null; 
        }
        Coroutine coroutineContactNoGround; IEnumerator Coroutine_ContactNoGround(){ 
            float time = 0;
            float second = 0;
            float milisecond = 0;

            dataReference.textCount.gameObject.SetActive(true);

            while(time < dataConfiguration.timeContactNoGround){
                
                time+=Time.deltaTime;

                if(second !=  Mathf.FloorToInt(time % 60)){ second =  Mathf.FloorToInt(time % 60);
                    dataReference.textCount.transform.DOKill(false); dataReference.textCount.transform.localScale = new Vector3(1,1,1);
                    dataReference.textCount.transform.DOShakeScale(second < 3 ? 0.1f : 0.5f).OnComplete(()=> { dataReference.textCount.transform.localScale = new Vector3(1,1,1); });
                }

                if(second != dataConfiguration.timeContactNoGround) {
                    milisecond = Mathf.FloorToInt((time * 1000000) % 100);
                    dataReference.textCount.SetText(string.Format("{0}<size=50%>.{1} ", second, milisecond));
                } else { dataReference.textCount.SetText("3"); }

                yield return null;  
            }
            
            dataRead.inTurn = true; 
            EffectManager.main.InstanceEffect("PopUp", dataReference.pivotRoot.position).SetPopUp("Unstick", TypePopUp.action); 

            coroutineContactNoGround=null; 

            yield return new WaitForSeconds(.25f);
            dataRead.inStun = false; 
            dataReference.textCount.gameObject.SetActive(false);
        }
        
        private bool Get_Comprobation_ContactRoom( ) { return dataRead.currentRoom.configuration.idScene < dataRead.contactRoom.configuration.idScene; }
        private Vector3 Get_PositionInGrid(Vector2 positinCheck){ return dataReference.grid.CellToWorld(dataReference.grid.WorldToCell(positinCheck))+dataReference.grid.cellSize/2; }
        private bool Get_VectorIsSimilar(Vector2 check, Vector2 check2, float checkValue){ return Vector2.Dot(check.normalized, check2.normalized) >= Math.Clamp(checkValue, -1, 1); }
        private float Get_MapedClamp01(float value, float minValue, float maxValue)  { return (value - minValue) / (maxValue - minValue); }
        
        [System.Serializable] public class DataReference {
            [Header("About Collision")]
            public Grid grid;
            public CheckCollision checkCollStage, checkBody;
            public Transform pivot, pivotRoot, pivotCenter;
            public TextMeshPro textCount;

            [Header("About Gravity")]
            public Rigidbody2D rigid;

            [Header("About State")]
            public Animator anim;
        }
        [System.Serializable] public class DataConfiguration {
            [Header("About Collision")]
            public Vector3 directionToCheck =  new Vector3(.1f ,.1f);
            public float timeStunForFall = 1;
            public float timeContactNoGround = 1;
            [Range(-1, 1)]public float vectorSimilitud_DetectedContact = 0.5f;
            [Range(-1, 1)]public float vectorSimilitud_DetectedHit = 0f;
            public float distanceToFixedPosition = 1;

            [Header("About Gravity")]
            public float gravity = 2;
            [Range(0,1)] public float gravityUp = .5f, gravityDown = 1f, gravityStun = 0.2f;
            [Range(0,1)] public float frictionGround = 0.9f;
            public Vector2 maxSpeed;
            public float scaleDeformation = .25f;
            
        }
        [System.Serializable] public class DataRead {
            [Header("About State")]
            public bool inStun;
            public bool inTurn;
            public bool isMirror;

            [Header("About Room")]
            public DataRoom currentRoom;
            public DataRoom contactRoom;

            [Header("About Collision")]
            public bool inContact;
            public Collider2D collStage;
            public Vector3 positionBody;
            public Vector3 positionCenterToCheck;
            public Vector3 positionContact;
            public Vector2 directionContact;
            public Vector2 directionVelocity;
            public Vector2 directionCheckColl;
            public float angleCheckColl;
            public float distanceToMove;
            public RaycastHit2D hit1, hit2, hitCentral, hitContact;
            public bool isHitted;

            [Header("About Gravity")]
            public bool inAir;
            public float timeInFall;

            [Header("About Game")]
            [HideInInspector] public bool isSaved;
        }
    }
    [System.Serializable] public class State : SystemElement_Player {
        public DataReference dataReference;
        public DataConfiguration dataConfiguration;
        public DataRead dataRead;

        public override void Initial(Element scr)  { base.Initial(scr); Show_Health(); }
        public override void Check () {  }
        public override void Apply() {  }

        public bool SetInvulnerable(bool state) { dataConfiguration.isInvulnerable = state; return dataConfiguration.isInvulnerable; }
        public bool GetInvulnerable() { return dataConfiguration.isInvulnerable; }
        
        public int ModifyHealth(int count) {
            if(count<0) { if(dataConfiguration.isInvulnerable) { count = 0; } }
            dataRead.health = Math.Clamp(dataRead.health + count, 0, dataConfiguration.healthMax);  Show_Health();
            if(dataRead.health==0) scrPlayer.System_actions.Action_Death();
            return dataRead.health; 
        }
        public void RestoreHealth(){ dataRead.health=dataConfiguration.healthMax; Show_Health(); }
        private void Show_Health() { CanvaManager.main.life.Show_Health(dataRead.health, dataConfiguration.healthMax); }
        
        public void ChangeWeapon (int count) { 
            if(dataRead.weapons.Count > 1) { 
                dataRead.idWeapon += count; 
                if(dataRead.idWeapon >= dataRead.weapons.Count) dataRead.idWeapon = 1;  
            } 
            SetWeapon(); 
        }
        public bool ChangeCheckpoint (int count) { 
            if(dataRead.checkpoints.Count > 1) {
                dataRead.idCheckpoint += count; if(dataRead.idCheckpoint>=dataRead.checkpoints.Count) dataRead.idCheckpoint = 0;
                scrPlayer.transform.position = LoadCheckpoint();
                CameraManager.main.SetOffset(Vector3.zero);
                return true;
            }
            else { return dataRead.checkpoints.Count > 1; }
        }
        
        public void Add_Checkpoint(DataCheckpoint newCheckpoint) { dataRead.checkpoints.Add(newCheckpoint); dataRead.idCheckpoint = dataRead.checkpoints.Count-1; }
        public void Add_Weapon(DataWeapon newWeapon) { dataRead.weapons.Add(newWeapon); dataRead.idWeapon = dataRead.weapons.Count-1; SetWeapon(); }
        public void Add_Achievement(DataAchievement newAchievement) { dataRead.achievements.Add(newAchievement); }

        public bool Get_Achievement (DataAchievement newAchievement) { return dataRead.achievements.Contains(newAchievement); }
        public DataWeapon GetWeapon () { if(dataRead.weapons.Count>0) { return dataRead.weapons[dataRead.idWeapon]; } else { return null; } }
        public bool is_CheckpointCurrent ( DataCheckpoint newCheckpoint) { if(dataRead.checkpoints.Count>0) return dataRead.checkpoints[dataRead.idCheckpoint] == newCheckpoint; else return false; }
        public bool is_Checkpoint ( DataCheckpoint checkCheckpoint) { return dataRead.checkpoints.Contains(checkCheckpoint); }
        public DataCheckpoint GetCheckpoint () { if(dataRead.checkpoints.Count>0) return dataRead.checkpoints[dataRead.idCheckpoint]; else return null; }
        

        public void SetWeapon() { 
            dataReference.sprWeapon.sprite = dataRead.weapons[dataRead.idWeapon].configuration.sprWeapon;
            if(dataRead.idWeapon!=0) { CanvaManager.main.notification.ShowNotification("Change " +  dataRead.weapons[dataRead.idWeapon].data.name , 1); }
            else {  CanvaManager.main.notification.ShowNotification("Unequiped" , 1); }
            CanvaManager.main.weapon.Show_Weapon(dataRead.weapons[dataRead.idWeapon].data.illustration);
        }
        public void SetCheckpoint(DataCheckpoint dataCheckpoint) { 
            if(!dataRead.checkpoints.Contains(dataCheckpoint)) { Add_Checkpoint(dataCheckpoint); }
            else { dataRead.idCheckpoint=0; foreach( DataCheckpoint data in dataRead.checkpoints ) { if(data==dataCheckpoint) { break; } dataRead.idCheckpoint++; } }
        }
        
        public Vector3 LoadCheckpoint(){
            SceneController.main.SetActiveScene(GetCheckpoint().configuration.currentRoom);
            return GetCheckpoint().configuration.positionWorld; 
        }

        [System.Serializable] public class DataReference {
            public SpriteRenderer sprWeapon;
        }
        [System.Serializable] public class DataConfiguration {
            [Header("About Health")]
            public int healthMax;
            public bool isInvulnerable;
        }
        [System.Serializable] public class DataRead {
            [Header("About Character")]
            public string name;
            
            [Header("About Health")]
            public int health;

            [Header("About Stage")]
            public int idCheckpoint = 0; public List<DataCheckpoint> checkpoints; 

            [Header("About Inventory")]
            public int idWeapon = 0; public List<DataWeapon> weapons; 
            public List<DataAchievement> achievements;
        }
    }
    [System.Serializable] public class Actions : SystemElement_Player {
        public DataReference dataReference;
        public DataConfiguration dataConfiguration;
        public DataRead dataRead;

        public override void Check () { 
            Check_State(); 
            if(dataConfiguration.useGamepad) Check_Input_Gamepad(); else Check_Input_Mouse(); 
        }
        public override void Apply() {
            Apply_ShowTrajectory(dataRead.inAim && dataRead.canJump); 
            if(Input.GetKeyDown(KeyCode.A)) {  }
        }

        private void Check_State(){
            dataRead.inStun = scrPlayer.System_gravity.dataRead.inStun;
            dataRead.inAir = scrPlayer.System_gravity.dataRead.inAir;
            dataRead.inTurn = scrPlayer.System_gravity.dataRead.inTurn;
            dataRead.angleCharacter = scrPlayer.System_gravity.dataRead.angleCheckColl;
        }
        
        private void Check_Input_Gamepad(){
            
            dataRead.inputDown = Input.GetButtonDown("Fire1");
            dataRead.inputStay = Input.GetButton("Fire1");
            dataRead.inputUp = Input.GetButtonUp("Fire1");


            /*
            dataRead.inputUp = dataRead.inputDown = false;
            if(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) != Vector2.zero) { 
                if(!dataRead.inputStay ) { dataRead.inputStay = true;  dataRead.inputDown = true;  }
            } else {
                if(dataRead.inputStay) { dataRead.inputStay = false; dataRead.inputUp = false; }
            }
            */

            if(!dataRead.inStun) {
                if(!dataRead.inAir){    
                    if (dataRead.inputDown) { 
                        dataRead.inAim = true;
                        dataRead.posMouseInitial = Vector2.zero;
                        dataRead.magnitudTimer = 0;
                    }
                    if (dataRead.inAim && dataRead.inputStay)  {
                        dataRead.posMouseFinal = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                        dataRead.directionAim = (dataRead.posMouseFinal - dataRead.posMouseInitial).normalized;
                        
                        dataRead.angleClamped = Vector3.SignedAngle(Vector3.down, dataRead.directionAim, Vector3.forward);
                        //dataRead.angleClamped = (dataRead.angleClamped < 0) ? 360f + dataRead.angleClamped : dataRead.angleClamped;
                        //dataRead.angleCharacter = (dataRead.angleCharacter < 0) ? 360f + dataRead.angleCharacter : dataRead.angleCharacter;
                        dataRead.angleClamped = Math.Clamp(dataRead.angleClamped, -dataConfiguration.angleLimits + dataRead.angleCharacter, dataConfiguration.angleLimits + dataRead.angleCharacter);
                        dataRead.directionAim = Quaternion.Euler(0, 0, dataRead.angleClamped) * Vector3.down;

                        dataRead.magnitudTimer += Time.deltaTime;

                        dataRead.magnitude = Mathf.Clamp(
                            (dataRead.posMouseFinal - dataRead.posMouseInitial).magnitude * dataConfiguration.forceMagnitude * Math.Clamp(dataRead.magnitudTimer, 0, dataConfiguration.forceMagnitudeTimer) / dataConfiguration.forceMagnitudeTimer, 
                            dataConfiguration.minMagnitude, dataConfiguration.maxMagnitude);

                        dataReference.pivotArrow[1].transform.rotation =  Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, dataRead.directionAim.normalized));
                        
                        dataRead.magnitudeFixed = ( dataRead.magnitude - dataConfiguration.minMagnitude) / (dataConfiguration.maxMagnitude - dataConfiguration.minMagnitude);
                        dataRead.canJump = (dataRead.posMouseFinal - dataRead.posMouseInitial).magnitude * dataConfiguration.forceMagnitude  > dataConfiguration.inputMinMagnitud && dataRead.inAim;
                        
                        if(dataRead.canJump) { 
                            if(!dataReference.pivotArrow[0].gameObject.activeSelf) dataReference.pivotArrow[0].gameObject.SetActive(true);
                            if(!dataReference.pivotArrow[2].gameObject.activeSelf) dataReference.pivotArrow[2].gameObject.SetActive(true);

                            dataReference.textForce.SetText(dataRead.magnitudeFixed<0.2f? "Min" : (dataRead.magnitudeFixed>.95f? "Max" : dataRead.magnitudeFixed.ToString("0.0")));

                            dataReference.pivotArrow[1].localScale = Vector3.one * Math.Clamp( 1.5f *dataRead.magnitudeFixed, .75f, 1.65f);

                            CameraManager.main.SetOffset(dataRead.directionAim * (dataRead.magnitude / dataConfiguration.maxMagnitude) * dataConfiguration.forceOffsetCamera);
                        } 
                        else {
                            if(dataReference.pivotArrow[2].gameObject.activeSelf) dataReference.pivotArrow[2].gameObject.SetActive(false);
                            if(dataReference.pivotArrow[0].gameObject.activeSelf) dataReference.pivotArrow[0].gameObject.SetActive(false);
                            dataReference.textForce.SetText("Tap");
                        }
                    }
                    if (dataRead.inAim && dataRead.inputUp) {
                        dataRead.inAim = false;
                        dataReference.pivotArrow[2].gameObject.SetActive(false);
                        dataReference.pivotArrow[0].gameObject.SetActive(false);
                        CameraManager.main.SetOffset(Vector3.zero);
                        if(dataRead.magnitude > dataConfiguration.minMagnitude)  Action_Deliz();
                    }
                    if(!dataRead.canJump && dataRead.inputUp) Action_TapGround();
                } else { if(dataRead.inTurn && dataRead.inputUp) { Action_TapAir(); }  }
            }
            dataReference.anim.SetBool("Jump", dataRead.inAim && dataRead.inputStay && dataRead.canJump);
            dataRead.inputBtn = dataRead.inputUp && dataRead.canJump;
        }
        private void Check_Input_Mouse(){
            var getInput = InputManager.main.GetMouse_Input(); 
            dataRead.inputDown = getInput == InputManager.TypeMouse.Down;
            dataRead.inputStay = getInput == InputManager.TypeMouse.Stay;
            dataRead.inputUp = getInput == InputManager.TypeMouse.Out;

            if(dataRead.inputUp) { CameraManager.main.SetOffset(Vector3.zero); }

            if(!dataRead.inStun) {
                if(!dataRead.inAir){    
                    if (!dataRead.inAim && dataRead.inputDown) { 
                        dataRead.inAim = true;

                        dataRead.posMouseInitial = Input.mousePosition; dataRead.posMouseInitial.z = 10;
                        dataRead.posMouseInitial = Camera.main.ScreenToViewportPoint(dataRead.posMouseInitial);

                        dataReference.pivotCursor[0].gameObject.SetActive(true);
                        dataReference.pivotCursor[0].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                    }
                    if (dataRead.inAim && dataRead.inputStay)  {
                        dataRead.posMouseFinal = Input.mousePosition; dataRead.posMouseFinal.z = 10;
                        dataRead.posMouseFinal = Camera.main.ScreenToViewportPoint(dataRead.posMouseFinal);

                        dataRead.directionAim = (dataRead.posMouseFinal - dataRead.posMouseInitial).normalized;
                        
                        dataRead.angleClamped = Vector3.SignedAngle(Vector3.down, dataRead.directionAim, Vector3.forward);
                        //dataRead.angleClamped = (dataRead.angleClamped < 0) ? 360f + dataRead.angleClamped : dataRead.angleClamped;
                        //dataRead.angleCharacter = (dataRead.angleCharacter < 0) ? 360f + dataRead.angleCharacter : dataRead.angleCharacter;
                        dataRead.angleClamped = Math.Clamp(dataRead.angleClamped, -dataConfiguration.angleLimits + dataRead.angleCharacter, dataConfiguration.angleLimits + dataRead.angleCharacter);
                        dataRead.directionAim = Quaternion.Euler(0, 0, dataRead.angleClamped) * Vector3.down;

                        //if(Vector2.Dot(dataRead.directionAim, Vector2.up) >= .8f) { dataRead.directionAim = Vector2.up; }

                        dataRead.magnitude = Mathf.Clamp((dataRead.posMouseFinal - dataRead.posMouseInitial).magnitude * dataConfiguration.forceMagnitude, dataConfiguration.minMagnitude, dataConfiguration.maxMagnitude);

                        dataReference.pivotCursor[1].transform.position = dataReference.pivotCursor[0].transform.position + dataRead.directionAim * dataRead.magnitude;
                        dataReference.pivotCursor[2].transform.position = dataReference.pivotCursor[0].transform.position + (dataRead.posMouseFinal - dataRead.posMouseInitial).normalized * dataRead.magnitude;
                        dataReference.pivotArrow[1].transform.rotation =  Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, dataRead.directionAim.normalized));
                        
                        dataReference.lineRender.SetPosition(0, dataReference.pivotCursor[1].transform.position); 
                        dataReference.lineRender.SetPosition(1, dataReference.pivotCursor[0].transform.position); 

                        dataRead.magnitudeFixed = ( dataRead.magnitude - dataConfiguration.minMagnitude) / (dataConfiguration.maxMagnitude - dataConfiguration.minMagnitude);
                        dataRead.canJump = (dataRead.posMouseFinal - dataRead.posMouseInitial).magnitude * dataConfiguration.forceMagnitude  > dataConfiguration.inputMinMagnitud && dataRead.inAim;
                        
                        if(dataRead.canJump) { 
                            if(!dataReference.pivotCursor[1].gameObject.activeSelf) dataReference.pivotCursor[1].gameObject.SetActive(true);
                            if(!dataReference.pivotCursor[2].gameObject.activeSelf) dataReference.pivotCursor[2].gameObject.SetActive(true);
                            if(!dataReference.pivotArrow[0].gameObject.activeSelf) dataReference.pivotArrow[0].gameObject.SetActive(true);
                            if(!dataReference.pivotArrow[2].gameObject.activeSelf) dataReference.pivotArrow[2].gameObject.SetActive(true);
                            if(!dataReference.lineRender.gameObject.activeSelf) dataReference.lineRender.gameObject.SetActive(true);

                            dataReference.textForce.SetText(dataRead.magnitudeFixed<0.2f? "Min" : (dataRead.magnitudeFixed>.95f? "Max" : dataRead.magnitudeFixed.ToString("0.0")));

                            dataReference.pivotCursor[0].localScale = Vector3.one * Math.Clamp(1.25f * dataRead.magnitudeFixed, .75f, 1.6f);
                            dataReference.pivotArrow[1].localScale = Vector3.one * Math.Clamp( 1.5f *dataRead.magnitudeFixed, .75f, 1.65f);

                            CameraManager.main.SetOffset(dataRead.directionAim * (dataRead.magnitude / dataConfiguration.maxMagnitude) * dataConfiguration.forceOffsetCamera);
                        } 
                        else {
                            if(dataReference.pivotCursor[1].gameObject.activeSelf) dataReference.pivotCursor[1].gameObject.SetActive(false);
                            if(dataReference.pivotCursor[2].gameObject.activeSelf) dataReference.pivotCursor[2].gameObject.SetActive(false);
                            if(dataReference.pivotArrow[2].gameObject.activeSelf) dataReference.pivotArrow[2].gameObject.SetActive(false);
                            if(dataReference.pivotArrow[0].gameObject.activeSelf) dataReference.pivotArrow[0].gameObject.SetActive(false);
                            if(dataReference.lineRender.gameObject.activeSelf) dataReference.lineRender.gameObject.SetActive(false);
                            dataReference.textForce.SetText("Tap");
                        }
                    }
                    if (dataRead.inAim && dataRead.inputUp) {
                        dataRead.inAim = false;
                        dataReference.pivotCursor[0].gameObject.SetActive(false);
                        dataReference.pivotCursor[1].gameObject.SetActive(false);
                        dataReference.pivotCursor[2].gameObject.SetActive(false);
                        dataReference.pivotArrow[2].gameObject.SetActive(false);
                        dataReference.pivotArrow[0].gameObject.SetActive(false);
                        dataReference.lineRender.gameObject.SetActive(false);
                        CameraManager.main.SetOffset(Vector3.zero);
                        if(dataRead.magnitude > dataConfiguration.minMagnitude)  Action_Deliz();
                    }
                    if(!dataRead.canJump && dataRead.inputUp) Action_TapGround();
                } 
                else {  if(dataRead.inTurn && dataRead.inputUp) { Action_TapAir(); }   }
                
            }
            else { 
                if(dataRead.inAim) {
                    dataRead.inAim = false;
                    dataReference.pivotCursor[0].gameObject.SetActive(false);
                    dataReference.pivotCursor[1].gameObject.SetActive(false);
                    dataReference.pivotCursor[2].gameObject.SetActive(false);
                    dataReference.pivotArrow[2].gameObject.SetActive(false);
                    dataReference.pivotArrow[0].gameObject.SetActive(false);
                    dataReference.lineRender.gameObject.SetActive(false);
                    CameraManager.main.SetOffset(Vector3.zero);
                } 
            }
            dataReference.anim.SetBool("Jump", dataRead.inAim && dataRead.inputStay && dataRead.canJump);
            dataRead.inputBtn = dataRead.inputUp && dataRead.canJump;
        }

        private void Apply_ShowTrajectory(bool showTrajectory) {
            dataReference.lineTrajectory.gameObject.SetActive(showTrajectory);
            if(showTrajectory) { 
                dataRead.trajectoryPointsList = new List<Vector3>();
                Vector2 initialPosition = dataReference.pivotInitialArrow.position;
                Vector2 initialVelocity = -dataRead.directionAim * dataRead.magnitude * dataConfiguration.forceJump;
                
                int id=0;
                while(!(id>dataConfiguration.trajectoryStepCount)) {
                    float time = id * dataConfiguration.trajectoryTimeStep;

                    bool isDown = false;
                    if(dataRead.trajectoryPointsList.Count>0) isDown = dataRead.trajectoryPointsList[Math.Clamp(id-1, 0, dataRead.trajectoryPointsList.Count-1)].y < dataRead.trajectoryPointsList[Math.Clamp(id-2, 0, dataRead.trajectoryPointsList.Count-1)].y;
                    
                    Vector2 position = initialPosition + initialVelocity * time + 0.5f * ( Physics2D.gravity * scrPlayer.System_gravity.dataConfiguration.gravity * (isDown ? scrPlayer.System_gravity.dataConfiguration.gravityUp : scrPlayer.System_gravity.dataConfiguration.gravityUp)) * time * time;

                    dataRead.trajectoryPointsList.Add(position);
                    
                    id++;

                    if(id>dataConfiguration.trajectoryStepCount) break; 
                }
                dataReference.lineTrajectory.positionCount=dataRead.trajectoryPointsList.Count;
                dataReference.lineTrajectory.SetPositions(dataRead.trajectoryPointsList.ToArray());
                
                dataReference.pivotResultTrajectory.position = dataRead.trajectoryPointsList[dataRead.trajectoryPointsList.Count-1];

                dataReference.SprResultTrajectory.sprite = dataReference.imgResultTrajectory[0];
                Vector2 directionResult = dataRead.trajectoryPointsList[dataRead.trajectoryPointsList.Count-2] -dataRead.trajectoryPointsList[dataRead.trajectoryPointsList.Count-1];
                dataReference.pivotResultTrajectory.rotation = Quaternion.Euler(0,0, Vector3.SignedAngle(Vector3.down, directionResult, Vector3.forward));
                dataReference.pivotResultTrajectory.localScale = Vector3.one * (1 + dataRead.magnitudeFixed * .5f);
            }
        }

        public void Action_Deliz() {
            scrPlayer.System_gravity.Set_Velocity(-dataRead.directionAim * dataRead.magnitude * dataConfiguration.forceJump); 
            scrPlayer.System_gravity.Set_StateTurn(true);
            scrPlayer.System_audio.PlaySFX("Jump");
            EffectManager.main.InstanceEffect("PopUp", dataReference.pivotRoot.position).SetPopUp("Jump", TypePopUp.action);
            EffectManager.main.InstanceEffect("Dust_Jump", scrPlayer.System_gravity.dataRead.positionContact).SetDust("Normal").SetRotation(dataRead.angleClamped); 
        }
        public void Action_TapAir() {
            scrPlayer.System_audio.PlaySFX("JumpAir");
            switch(scrPlayer.System_state.GetWeapon().configuration.type) {
                case DataWeapon.Configuration.TypeAction.None: 
                    EffectManager.main.InstanceEffect("PopUp", dataReference.pivotRoot.position).SetPopUp("None", TypePopUp.action);
                    break;
                case DataWeapon.Configuration.TypeAction.Jump:
                    scrPlayer.System_gravity.Set_StateTurn(false);
                    scrPlayer.System_gravity.Set_Velocity(new Vector2(dataReference.rigid.velocity.x*0.5f, dataConfiguration.forceJump * dataConfiguration.forceDobleJump));
                    CameraManager.main.AddShake("Recover");
                    EffectManager.main.InstanceEffect("PopUp", dataReference.pivotRoot.position).SetPopUp("Jump", TypePopUp.action);
                    EffectManager.main.InstanceEffect("Dust_Jump", scrPlayer.System_gravity.dataRead.positionContact).SetDust("Normal").SetRotation(dataRead.angleClamped); 
                    break;
                case DataWeapon.Configuration.TypeAction.Stopped:
                    scrPlayer.System_gravity.Set_StateTurn(false);
                    scrPlayer.System_gravity.Set_Velocity(new Vector2(dataReference.rigid.velocity.x*0.25f, 1));
                    CameraManager.main.AddShake("Recover");
                    EffectManager.main.InstanceEffect("PopUp", dataReference.pivotRoot.position).SetPopUp("Stopped", TypePopUp.action);
                    break;
                case DataWeapon.Configuration.TypeAction.WallCling:
                    scrPlayer.System_gravity.Set_StateTurn(false);
                    CameraManager.main.AddShake("Recover");
                    EffectManager.main.InstanceEffect("PopUp", dataReference.pivotRoot.position).SetPopUp("Preparation", TypePopUp.action);
                    break;
            }
        }
        public void Action_TapGround() { 
            if (dataRead.interact != null) { dataRead.interact.Action_Interact(); } 
            // else { scrPlayer.System_state.ChangeWeapon(1); }
        }

        public void Action_Interact() { scrPlayer.System_audio.PlaySFX("Interact"); dataRead.interact.Action_Interact();  }
        public void Action_ChangeWeapon() { scrPlayer.System_state.ChangeWeapon(1); }

        public void Action_Spawn() { 
            dataReference.pivotRoot.position = scrPlayer.System_state.LoadCheckpoint(); 
            scrPlayer.System_state.RestoreHealth();
            scrPlayer.System_gravity.Set_Velocity(Vector2.up*10);
            LevelManager.main.systemManager.Spawn();
        }
        public void Action_Death() { 
            if (dataRead.interact!=null) { 
                switch(dataRead.interact.type){
                    case TypeInteractable.Checkpoint: LevelManager.main.systemInteract.systemCheckpoint.Set_CheckpointInteract(null); break;
                    case TypeInteractable.Item: LevelManager.main.systemInteract.systemItem.Set_ItemInteract(null);  break;
                    case TypeInteractable.NPC: LevelManager.main.systemInteract.systemDialogue.Set_Dialogue(null, null);  break;
                }
            }
            LevelManager.main.Function_Button_FLowToDeath();
        }
        public void Action_Hit(){
            if(!scrPlayer.System_gravity.dataRead.isHitted) { 
                scrPlayer.System_gravity.dataRead.isHitted = true; 
                scrPlayer.System_state.ModifyHealth(-1); 
                scrPlayer.System_gravity.Action_Hit(Vector2.zero);
            }
            scrPlayer.System_audio.PlaySFX("Hit");
            dataReference.anim.SetTrigger("Hit"); 
            EffectManager.main.InstanceEffect("Dust_Hit", dataReference.pivotRoot.position).SetDust("Normal"); 
            CameraManager.main.AddShake("Hit");
        }

        public void Set_Interaction(Interactable newInteract) {  dataRead.interact = newInteract;  }

        [System.Serializable] public class DataReference {
            [Header("About State")]
            public Animator anim;
            public Rigidbody2D rigid;
            public Transform pivotEffect;

            [Header("About Jump")]
            public Transform pivotRoot;
            public Transform[] pivotCursor;
            public Transform[] pivotArrow;
            public LineRenderer lineRender;
            public TextMeshProUGUI textForce;

            [Header("About Trajectory")]
            public Transform pivotInitialArrow;
            public Transform pivotResultTrajectory;
            public LineRenderer lineTrajectory;
            public SpriteRenderer SprResultTrajectory;
            public List<Sprite> imgResultTrajectory;
        }
        [System.Serializable] public class DataConfiguration {
            [Header("About Input")]
            public bool useGamepad = false;
            public float inputMinMagnitud = 0.25f;
            public float forceMagnitude = 10;
            public float forceMagnitudeTimer = 1;

            [Header("About Jump")]
            public float forceJump =1;
            public float forceDobleJump =1;
            
            public float maxMagnitude = 5;
            public float minMagnitude = 1f;
            public float angleLimits = 90f;

            [Header("About Trajectory")]
            public float forceOffsetCamera = 1;
            public float trajectoryTimeStep = 0.05f;
            public int trajectoryStepCount = 15;
        }
        [System.Serializable] public class DataRead {
            [Header("About Input")]
            public Vector2 inputDirection; 
            public float inputMagnitud;
            public bool inputBtn;
            public bool inputDown, inputStay, inputUp;
            public float magnitudTimer = 0;

            [Header("About State")]
            public bool inAir;
            public bool inStun;
            public bool inTurn;
            public float angleCharacter;

            [Header("About Jump")]
            public bool inAim; 
            public bool canJump;
            public Vector3 posMouseInitial = Vector2.zero;
            public Vector3 posMouseFinal = Vector2.zero;
            public Vector3 posMouseInitialVIEWPORT = Vector2.zero;
            public Vector3 posMouseFinalVIEWPORT = Vector2.zero;
            public Vector3 directionAim = Vector2.zero;
            public float magnitude;
            public float magnitudeFixed = 0;
            public float angleClamped;


            [Header("About Trajectory")]
            public List<Vector3>  trajectoryPointsList = new List<Vector3>();

            [Header("About Interact")]
            public Interactable interact;
            
        }
    }
    [System.Serializable] public class Audio {
        [Header("Reference")]
        public AudioObject_ToOwnSource scrAudio;

        public void PlaySFX(string clip) { scrAudio.PlaySFX(clip); }
    }
}
