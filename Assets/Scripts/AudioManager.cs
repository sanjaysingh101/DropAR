using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioMixerGroup mixerGroup;
    public GameObject audiolistner;



    public Sound[] sounds;
    private void Start()
    {
        Play("Theme");
    }


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = mixerGroup;
        }
    }
    public void mute()
    {
        AudioListener.pause = true;
        audiolistner.GetComponent<AudioListener>().enabled = false;
        GameObject.DontDestroyOnLoad(gameObject);


    }
    public void unmute()
    {
        AudioListener.pause = false;
        audiolistner.GetComponent<AudioListener>().enabled = true;
        GameObject.DontDestroyOnLoad(gameObject);
    }


    public void Play(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Play();
    }

}
