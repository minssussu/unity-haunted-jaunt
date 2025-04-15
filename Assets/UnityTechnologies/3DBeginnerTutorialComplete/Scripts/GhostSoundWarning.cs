using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSoundWarning : MonoBehaviour
{
    public Transform player;
    public float warningDistance = 5.0f;
    private AudioSource m_AudioSource;
    private AudioClip m_GhostSound;
    private float m_LastWarningTime;
    private float m_WarningCooldown = 3.0f;
    private bool m_IsPlayerInRange = false;
    
    void Start()
    {
        // Find player if not set
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
        // Create audio source if needed
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_AudioSource.spatialBlend = 1.0f; // 3D sound
            m_AudioSource.volume = 0.5f;
            m_AudioSource.loop = false;
        }
        
        // Find the existing ghost sound from the project
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in audioSources)
        {
            if (source.clip != null && source.clip.name == "SFXGhostMove")
            {
                m_GhostSound = source.clip;
                break;
            }
        }
        
        // If we couldn't find it in scene, try to load it
        if (m_GhostSound == null)
        {
            m_GhostSound = Resources.Load<AudioClip>("UnityTechnologies/3DBeginnerTutorialComplete/Audio/SFXGhostMove");
        }
        
        if (m_GhostSound != null)
        {
            m_AudioSource.clip = m_GhostSound;
        }
        else
        {
            Debug.LogWarning("Could not find ghost sound effect!");
        }
    }
    
    void Update()
    {
        if (player == null || m_GhostSound == null)
            return;
            
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is within warning range
        bool wasInRange = m_IsPlayerInRange;
        m_IsPlayerInRange = distanceToPlayer <= warningDistance;
        
        // Only trigger when first entering range or after cooldown
        if (m_IsPlayerInRange && (!wasInRange || (Time.time - m_LastWarningTime > m_WarningCooldown)))
        {
            // Don't play if already playing
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
                m_LastWarningTime = Time.time;
            }
        }
    }
    
    // Visualize the warning distance in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, warningDistance);
    }
}