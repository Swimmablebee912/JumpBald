using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extra_TimerController : MonoBehaviour
{
    public Collider2D coll;
    public bool contactColl = false;

    private void OnTriggerEnter2D(Collider2D other) { contactColl=true; LevelManager.main.systemTimer.PauseTimer(true); }    
    private void OnTriggerStay2D(Collider2D other)  { if(contactColl!=false) { contactColl=true; LevelManager.main.systemTimer.PauseTimer(true); } }
    private void OnTriggerExit2D(Collider2D other) { contactColl= false; LevelManager.main.systemTimer.PauseTimer(false); }
}
