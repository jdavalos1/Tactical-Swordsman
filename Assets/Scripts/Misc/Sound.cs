using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _volume;
    [SerializeField] bool _loop;
    

    public float volume
    {
        set
        {
            _volume = value;
            source.volume = _volume;
        }
        get { return _volume; }
    }
    public bool loop
    {
        set
        {
            _loop = value;
            source.loop = _loop;
        }
        get { return _loop; }
    }

    [Range(0.1f, 3.0f)]
    public float pitch;
    public string name;

    [HideInInspector]
    public AudioSource source;

}