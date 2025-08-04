using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] private SoundEffectGroup[] m_soundEffectGroups;
    private Dictionary<string, List<AudioClip>> m_soundDictionary;

    void Awake()
    {
        InitializeDictionary();
    }

    void InitializeDictionary()
    {
        m_soundDictionary = new Dictionary<string, List<AudioClip>>();

        foreach (SoundEffectGroup soundEffectGroup in m_soundEffectGroups)
        {
            m_soundDictionary[soundEffectGroup.name] = soundEffectGroup.audioClips;
        }
    }

    public AudioClip GetRandomClip(string name)
    {
        if (m_soundDictionary.ContainsKey(name))
        {
            List<AudioClip> audioClips = m_soundDictionary[name];
            if (audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
        }

        return null;
    }
}

[System.Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
}
