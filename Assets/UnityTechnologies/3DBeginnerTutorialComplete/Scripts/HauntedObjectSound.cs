using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntedObjectSound : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 2.0f;
    public AudioClip[] soundEffects; // Array of possible sound effects to play
    public float volumeMax = 1.0f;
    public float volumeMin = 0.1f;
    public float pitchVariation = 0.1f; // Random pitch variation to add variety
    public ParticleSystem dustEffect; // Optional particle effect to trigger with sound
    
    private AudioSource m_AudioSource;
    private bool m_IsPlayerInRange = false;
    private float m_LastTriggerTime;
    private float m_TriggerCooldown = 5.0f; // Time between auto-triggers when player stays in range
    
    void Start()
    {
        // Create an AudioSource component if one doesn't already exist
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_AudioSource.spatialBlend = 1.0f; // Make the sound 3D
        }
        
        if (soundEffects == null || soundEffects.Length == 0)
        {
            Debug.LogWarning("No sound effects assigned to HauntedObjectSound on " + gameObject.name);
        }
    }
    
    void Update()
    {
        if (player == null)
            return;
            
        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Determine if player is in range
        bool wasInRange = m_IsPlayerInRange;
        m_IsPlayerInRange = distanceToPlayer <= triggerDistance;
        
        // Player just entered range
        if (m_IsPlayerInRange && !wasInRange)
        {
            TriggerHauntedEffect();
        }
        // Player is staying in range - occasional random triggers
        else if (m_IsPlayerInRange && Time.time - m_LastTriggerTime > m_TriggerCooldown)
        {
            // Random chance to trigger while in range
            if (Random.value < 0.3f) // 30% chance
            {
                TriggerHauntedEffect();
            }
            m_LastTriggerTime = Time.time;
        }
    }
    
    void TriggerHauntedEffect()
    {
        if (soundEffects == null || soundEffects.Length == 0)
            return;
            
        // Select a random sound from the array
        int randomIndex = Random.Range(0, soundEffects.Length);
        m_AudioSource.clip = soundEffects[randomIndex];
        
        // Set random volume and pitch for variety
        m_AudioSource.volume = Random.Range(volumeMin, volumeMax);
        m_AudioSource.pitch = 1.0f + Random.Range(-pitchVariation, pitchVariation);
        
        // Play the sound
        m_AudioSource.Play();
        
        // Trigger particle effect if assigned
        if (dustEffect != null)
        {
            dustEffect.Play();
        }
        
        m_LastTriggerTime = Time.time;
    }
    
    // Visualize the trigger range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}