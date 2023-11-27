using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject_ToOwnSource : MonoBehaviour
{
      [Header("SFX")]
    [SerializeField] SFX[] sfxClip;

    public void PlaySFX(string SFXName)
    {
        SFX sfxPlay = System.Array.Find(sfxClip, sfx => sfx.name == SFXName);
        if (sfxPlay != null) sfxPlay.PlayAudio();
    }
}
