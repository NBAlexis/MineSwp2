using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUISound
{
    EUS_Press,
    EUS_Error,
    EUS_Win,
    EUS_Lose,
    EUS_Tag,
    EUS_Explode,
}

public class ASound : MonoBehaviour
{
    public AudioSource m_pMusicSources;
    public AudioSource[] m_pSounds;

    public AudioClip m_pMusic;
    public AudioClip[] m_pEffect;
    public AudioClip[] m_pUI;

    private float m_fPitch = 1.0f;

    public static ASound Sound;

    public void Start()
    {
        Sound = this;
        m_fPitch = 1.0f;

        m_pMusicSources.clip = m_pMusic;
        m_pMusicSources.loop = true;
        m_pMusicSources.playOnAwake = true;
        m_pMusicSources.Play();
    }

    public void Update()
    {
        if (m_fPitch > 1.0f)
        {
            m_fPitch -= Time.deltaTime;
        }
    }

    private AudioSource FindSource()
    {
        for (int i = 0; i < m_pSounds.Length; ++i)
        {
            if (!m_pSounds[i].isPlaying)
            {
                return m_pSounds[i];
            }
        }

        return null;
    }

    public void PlayUISound(EUISound eSound)
    {
        AudioSource source = FindSource();
        if (null != source)
        {
            source.pitch = 1.0f;
            source.clip = m_pUI[(int) eSound];
            source.Play();
        }
    }

    public void PlayEffectSound(EGridEffect eSound)
    {
        AudioSource source = FindSource();
        if (null != source)
        {
            m_fPitch = Mathf.Clamp(m_fPitch + 0.1f, 1.0f, 3.0f);
            source.pitch = m_fPitch;
            source.clip = m_pEffect[(int)eSound];
            source.Play();
        }
    }
}
