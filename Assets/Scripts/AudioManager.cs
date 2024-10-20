using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private static AudioManager instance;
    AudioSource[] audioSources;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy the duplicate AudioManager
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = s.mixerGroup;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.name = s.name;
        }
    }
    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
        Play("BGMusic");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Play();
    }

    public void PauseMusic()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source.outputAudioMixerGroup.name == "Music")
            {
                source.Pause();
            }
        }
    }
    public void UnpauseMusic()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source.outputAudioMixerGroup.name == "Music")
            {
                source.UnPause();
            }
        }
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }
    bool AudioClipEquals(AudioClip clip1, AudioClip clip2)
    {
        return clip1 == clip2;
    }
}

