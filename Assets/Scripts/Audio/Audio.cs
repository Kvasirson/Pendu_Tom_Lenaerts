using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public enum AudioType
{
    MUSIC = 0,
    FX = 1
}

[System.Serializable]
public class Audio
{
    public string name;

    public AudioType audioType;

    [Space]
    [Space]
    [Space]

    public AudioClip[] clips;

    public int maxInstNb = 1;
    [HideInInspector]
    public int curInstNb = 1;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;
    public bool playOnAwake = false;

    [HideInInspector]
    public AudioSource source;

    [HideInInspector]
    public bool hasMultipleClips = false;
}
