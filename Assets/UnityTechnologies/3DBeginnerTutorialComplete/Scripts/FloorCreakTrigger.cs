using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreakTrigger : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private AudioClip m_CreakSound;
    private float m_LastPlayTime;
    private float m_MinTimeBetweenPlays = 0.5f;
    
    void Start()
    {
        // Create audio source
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_AudioSource.spatialBlend = 1.0f; // 3D sound
            m_AudioSource.volume = 0.3f;
            m_AudioSource.pitch = 0.7f; // Lower pitch for creaking sound
        }
        
        // Find an existing sound from the project to repurpose
        // We can use the candle sound for creaking floors by adjusting pitch
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in audioSources)
        {
            if (source.clip != null && source.clip.name == "SFXCandle")
            {
                m_CreakSound = source.clip;
                break;
            }
        }
        
        // Try to load it if not found
        if (m_CreakSound == null)
        {
            m_CreakSound = Resources.Load<AudioClip>("UnityTechnologies/3DBeginnerTutorialComplete/Audio/SFXCandle");
        }
        
        if (m_CreakSound != null)
        {
            m_AudioSource.clip = m_CreakSound;
        }
        else
        {
            Debug.LogWarning("Could not find sound effect for floor creak!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayCreakSound();
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        // Only play sound again if enough time has passed
        if (other.CompareTag("Player") && Time.time - m_LastPlayTime >= m_MinTimeBetweenPlays)
        {
            // Random chance to play while player is in trigger area
            if (Random.value < 0.2f) // 20% chance per check
            {
                PlayCreakSound();
            }
        }
    }
    
    void PlayCreakSound()
    {
        if (m_AudioSource != null && m_CreakSound != null && !m_AudioSource.isPlaying)
        {
            // Randomize pitch slightly for variation
            m_AudioSource.pitch = Random.Range(0.6f, 0.8f);
            m_AudioSource.Play();
            m_LastPlayTime = Time.time;
        }
    }
}