using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
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

    /// <summary>
    /// Play sound
    /// </summary>
    /// <param name="name">Sound to play</param>
    public void Play(string name)
    {
        Sound s = sounds.Find(s => s.name == name);
        if (s != null) s.source.Play();
    }
    /// <summary>
    /// Play sound with a delay first
    /// </summary>
    /// <param name="name">Name of sound</param>
    /// <param name="delay">Time to delay in seconds</param>
    public void Play(string name, float delay)
    {
        Sound s = sounds.Find(s => s.name == name);
        if(s != null)
        {
            StartCoroutine(DelaySound(s, delay));
        }
    }

    public void Stop(string name)
    {
        Sound s = sounds.Find(s => s.name == name);
        
        if(s != null) s.source.Stop();
    }

    private IEnumerator DelaySound(Sound s, float delay)
    {
        yield return new WaitForSeconds(delay);
        s.source.Play();
    }
}
