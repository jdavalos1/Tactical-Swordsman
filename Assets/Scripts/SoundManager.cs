using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<Sound> sounds;

    void Awake()
    {
        foreach(var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Play(string name)
    {
        Sound s = sounds.Find(s => s.name == name);
        if (s != null) s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = sounds.Find(s => s.name == name);
        
        if(s != null) s.source.Stop();
    }
}
