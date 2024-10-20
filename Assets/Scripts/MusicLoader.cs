using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicLoader : MonoBehaviour
{
    GameObject audioManager;
    AudioManager audioScript;
    [SerializeField] AudioClip music;
    public void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager");
        audioScript = audioManager.GetComponent<AudioManager>();
        AudioSource[] audioSources = audioManager.GetComponents<AudioSource>(); 

        foreach (AudioSource source in audioSources)
        {
            if (source.outputAudioMixerGroup.name == "Music" && music != source.clip)
            {
                source.clip = music;
                if (!source.isPlaying)
                {
                    audioScript.Play("BGMusic");
                }
            }
        }
    }
}
