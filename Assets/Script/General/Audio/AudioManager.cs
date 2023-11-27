using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    [Header("====SOURCEs====")]
    [SerializeField] MIXER[] mixers;
    [SerializeField] Source[] sources;
    [SerializeField] EFFECT[] effects;

    [Header("====CLIPs Generales====")]
    [SerializeField] SFX[] sfx;
    [SerializeField] MUSIC[] music;

    #region CONSTRUCT
    private void Awake() {
        if (main != null) Destroy(this.gameObject); main = this;
        Initializer();
    }
    public void Initializer() {
        foreach (Source sr in sources) { sr.source.outputAudioMixerGroup = sr.mixer; }
        foreach (MIXER mixer in mixers) { mixer.SetVolume(mixer.volume); }
    }

    public void LoadData(){
        
    }
    #endregion

    #region FUNCTIONS
    #region CLIPS
    public void PlaySFX(string nameAudio)
    {
        SFX sfx_ = Array.Find(sfx, sound => sound.name == nameAudio);
        if (sfx_ != null) sfx_.PlayAudio();
    }

    MUSIC currentMusic, musicToChange;
    public void PlayMUSIC(string nameAudio)
    {
        musicToChange = Array.Find(music, sound => sound.name == nameAudio);
        if (musicToChange != null) { if (musicToChange != currentMusic) { currentMusic = musicToChange; musicToChange.PlayAudio(); } }
        else if(nameAudio!="") { if(currentMusic!=null) { Array.Find(sources, source => source.source == (currentMusic.source)).source.Stop(); } currentMusic=null;}
    }
    #endregion
    #region EFFECTS
    public void ChangeSnapshot(string nameEffect, string nameSnapshot) {

        EFFECT mod = Array.Find(effects, effect => effect.name == nameEffect);
        if (mod != null) mod.SetActive(nameSnapshot);
    }
    #endregion
    #region VOLUME
    public void SetVolume(string nameMixer, float volume) {  Array.Find(mixers, sound => sound.nameMixer == nameMixer).SetVolume(volume);  }
    bool muteMusic = false;
    public bool MuteMusic() { muteMusic=!muteMusic; if(muteMusic) SetVolume("Music",0); else SetVolume("Music",1); return muteMusic; }
    public bool MuteMusic(bool mute) { muteMusic=mute; if(muteMusic) SetVolume("Music",0); else SetVolume("Music",1); return muteMusic; }
    #endregion
    #endregion

}

[Serializable] public class MIXER {
    
    public string nameMixer;
    [SerializeField] AudioMixer mixer;
    [SerializeField] string nameVarVolume;
    [SerializeField, Range(0.0001f, 1f)] float minVolume, maxVolume;
    [Range(0, 1f)] public float volume;

    public void SetEffect(string nameEffect, float value) { mixer.SetFloat(nameEffect, value); }
    public void SetVolume(float value) {
        volume = Mathf.Clamp(value, minVolume, maxVolume);
        mixer.SetFloat(nameVarVolume, Mathf.Log10(volume) * 20);
    }
} 
[Serializable] public class EFFECT
{
    public string name;
    public SNAPSHOT[] snapshots;

    public void SetActive(string nameSnapshot) { Array.Find(snapshots, snapshot => snapshot.name == nameSnapshot).ToTransition();  }
}
[Serializable] public class SNAPSHOT
{
    public string name;
    [Range(0, 1)] public float timeToReach;
    public AudioMixerSnapshot snapshots;

    public void ToTransition() { snapshots.TransitionTo(timeToReach);  }
}
[Serializable] public class Source
{
    public string name;
    public AudioMixerGroup mixer;
    public AudioSource source;

    public bool IsPlaying() {return source.isPlaying;}
}

#region AUDIO
public class Audio
{
    public string name;
    public AudioClip[] clip;
    public AudioSource source;

    [Range(0, 1)] public float volume = 1;

    public virtual void PlayAudio() { }
}

[Serializable] public class SFX : Audio
{
    [Range(0, 2)] public float pitch = 1;
    [Range(0, 1)] public float randomizerPitch = 0;
    [Range(0, 1)] public float delay = 0;

    public override void PlayAudio()  {
        if (clip.Length == 0 || clip == null) return;
        AudioManager.main.StartCoroutine(toPlayAudio());
    }
    public IEnumerator toPlayAudio() {
        yield return new WaitForSeconds(delay);
        source.clip = clip[UnityEngine.Random.Range(0, clip.Length)];
        source.volume = volume * volume;
        source.pitch = pitch + UnityEngine.Random.Range(-randomizerPitch, randomizerPitch);
        source.PlayOneShot(source.clip);
    }
}
[Serializable] public class MUSIC : Audio
{
    public override void PlayAudio()
    {
        if (clip.Length == 0 || clip==null) return;
        source.clip = clip[UnityEngine.Random.Range(0, clip.Length)];
        source.volume = volume * volume;
        source.Play();
    }
}

[System.Serializable]  public class SFX_sound
{
    public string name;
    public string nameDataBase;
    public void Play() { AudioManager.main.PlaySFX(nameDataBase); }
}
#endregion
