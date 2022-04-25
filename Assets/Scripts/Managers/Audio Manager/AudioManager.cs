using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundName
{
    WAVE_START,
    GATES_OPEN,
    SHOOT,
    BOUNCE,
    USE_ITEM,
    MARKET,
    GATES_CLOSE,
    HIT,
    SLASH,
    DASH,
    BGM_1,
    BGM_2,
    BGM_3,
    ARROW_PICKUP,
    WATER_BLOOP,
}
public class AudioManager : Singleton<AudioManager>
{
    public List<Sound> Sounds = new List<Sound>();

    public bool game;
    public bool tutorial;

    public  void Awake()
    {
        foreach (Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.outputAudioMixerGroup = s.Output;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
            s.Source.playOnAwake = s.PlayOnAwake;
        }
    }

    private void Start()
    {
        if(game)
            PlaySound(SoundName.BGM_1);

        if (tutorial)
            PlaySound(SoundName.BGM_2);
    }

    public void PlaySound(SoundName name)
    {
        Sound selectedSound = Sounds.Find(s => s.Name == name);

        if (selectedSound.PlayOneShot)
        {
            selectedSound.Source.PlayOneShot(selectedSound.Clip);
        }
        else
        {
            selectedSound.Source.Play();
        }
    }

    public void StopSound(SoundName name)
    {
        Sounds.Find(s => s.Name == name).Source.Stop();
    }

}

[System.Serializable]
public class Sound
{
    public SoundName Name;
    public AudioClip Clip;
    public AudioMixerGroup Output;
    public bool PlayOneShot;
    public bool Loop;
    public bool PlayOnAwake;
    [Range(0f, 1f)] public float Volume;
    [Range(-3f, 3f)] public float Pitch;
    [HideInInspector] public AudioSource Source;
}