using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager main; private void Awake()  { if(main==null) { main=this; } else { Destroy(gameObject); } }

    [Header("Configuration")]
    public float time = 0.15f;

    [Header("Reference")]
    public bool input = false;

    public TypeMouse type; public enum TypeMouse { None, Down, Stay, Out }

    public void SetMouse_Input(bool newInput) { 
        if(coroutine != null) StopCoroutine(coroutine); 
        coroutine = StartCoroutine(CoroutineInput(newInput)); 
    }
    public TypeMouse GetMouse_Input() { 
        switch(type){
            case TypeMouse.Down: if(coroutine != null) StopCoroutine(coroutine);  type = TypeMouse.Stay; return TypeMouse.Down; 
            case TypeMouse.Out: if(coroutine != null) StopCoroutine(coroutine);  type = TypeMouse.None; return TypeMouse.Out; 
            case TypeMouse.Stay: return TypeMouse.Stay; 
            default: return TypeMouse.None; 
        }
    }

    Coroutine coroutine;
    private IEnumerator CoroutineInput( bool stateInput) { 
        input = stateInput;

        if(input) { type = TypeMouse.Down; } 
        else { type = TypeMouse.Out; }

        yield return new WaitForSeconds(time);

        if(input) { type = TypeMouse.Stay; }
        else { type = TypeMouse.None; }
    }

}
