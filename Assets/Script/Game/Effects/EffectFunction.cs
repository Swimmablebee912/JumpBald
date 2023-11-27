using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffectFunction : ObjectPool
{

    public int timeToHide = 1;

    public Transform thisTransform;
    public TextMeshPro text_;
    public SpriteRenderer spr, sprSecondary;
    public Animator anim;

    private void OnEnable() {
        if(anim!=null) anim.SetTrigger("Spawn");
        StartCoroutine(Desactivate());
    }
    private void OnDisable() {
        
    }

    public void SetText(string text) { text_.SetText(text); }
    public void SetColor(Color color) {
        if(spr!=null) spr.color=color;
        if(sprSecondary!=null) sprSecondary.color=color;
        else if(text_!=null) text_.color=color;
    }
    public void SetSpeed(float speed) { if(anim!=null) anim.SetFloat("Speed", speed);  }

    public void SetPosition(Vector3 position) { if(thisTransform!=null) thisTransform.position+=position; }
    public void SetAngle(float angle) { if(thisTransform!=null) thisTransform.rotation=Quaternion.Euler(0,0,angle);  }
    public void SetScale(Vector3 scale) { if(thisTransform!=null) thisTransform.localScale=scale;  }

    public virtual IEnumerator Desactivate(){
        yield return new WaitForSecondsRealtime(timeToHide);
        HideObject();
    }

    public void HideObject() {  EffectManager.main.DestroyEffect(this.gameObject); }
}
