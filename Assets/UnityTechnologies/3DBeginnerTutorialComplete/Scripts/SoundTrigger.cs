using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public AudioClip soundEffect; // The sound to play when triggered
    public float volume = 1.0f; // Volume level for the sound
    public float minTimeBetweenPlays = 3.0f; // Minimum time between sound triggers
    public bool playOnce = false; // If true, the sound will only play once
    public float triggerCooldown = 3.0f; // Time before the trigger can be activated again
    
    private AudioSource m_AudioSource;
    private bool m_HasPlayed = false;
    private float m_LastPlayTime;
    
    void Start()
    {
        // Create an AudioSource component if one doesn't already exist
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_AudioSource.spatialBlend = 1.0f; // Make the sound 3D
            m_AudioSource.volume = volume;
        }
        
        // Set the audio clip
        if (soundEffect != null)
        {
            m_AudioSource.clip = soundEffect;
        }
        else
        {
            Debug.LogWarning("No sound effect assigned to SoundTrigger on " + gameObject.name);
        }
        
        m_LastPlayTime = -minTimeBetweenPlays; // Allow immediate play on first trigger
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            TriggerSound();
        }
    }
    
    public void TriggerSound()
    {
        // If set to play only once and has already played, do nothing
        if (playOnce && m_HasPlayed)
            return;
        
        // Check if enough time has passed since the last play
        if (Time.time - m_LastPlayTime >= minTimeBetweenPlays)
        {
            // Play the sound
            if (m_AudioSource != null && m_AudioSource.clip != null)
            {
                m_AudioSource.Play();
                m_HasPlayed = true;
                m_LastPlayTime = Time.time;
            }
        }
    }
    
    // Optional: Add a method to reset the play-once flag
    public void ResetPlayOnceFlag()
    {
        m_HasPlayed = false;
    }
}