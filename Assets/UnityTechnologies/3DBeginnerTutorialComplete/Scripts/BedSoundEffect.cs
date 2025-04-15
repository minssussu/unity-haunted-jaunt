using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedSoundEffect : MonoBehaviour
{
    public AudioClip teleportSuccessSound; // Sound to play when player gets the 1/10 chance to go to end
    public AudioClip teleportFailSound;   // Sound to play when player rises and goes to start
    public float volume = 1.0f;
    
    private AudioSource m_AudioSource;
    
    void Start()
    {
        // Create an AudioSource component if one doesn't already exist
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_AudioSource.spatialBlend = 0.0f; // Make the sound 2D (follows player)
            m_AudioSource.volume = volume;
        }
    }
    
    // Method to be called from PlayerMovement.cs when bed teleport happens
    public void PlayTeleportSound(bool isSuccessful)
    {
        if (m_AudioSource != null)
        {
            // Set the appropriate clip based on teleport success/failure
            m_AudioSource.clip = isSuccessful ? teleportSuccessSound : teleportFailSound;
            
            // Play the sound if we have a valid clip
            if (m_AudioSource.clip != null)
            {
                m_AudioSource.Play();
            }
        }
    }
}