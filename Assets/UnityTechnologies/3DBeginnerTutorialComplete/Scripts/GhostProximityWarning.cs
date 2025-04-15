using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostProximityWarning : MonoBehaviour
{
    public Transform player;
    public float warningDistance = 5.0f;
    public AudioClip proximityWarningSound;
    public float minTimeBetweenWarnings = 2.0f;
    
    private AudioSource m_AudioSource;
    private float m_LastWarningTime;
    private bool m_HasPlayedWarning;
    
    void Start()
    {
        // Create an AudioSource component if one doesn't already exist
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_AudioSource.spatialBlend = 0.0f; // Make it 2D for a clear warning
            m_AudioSource.volume = 0.5f;
            m_AudioSource.loop = false;
        }
        
        if (proximityWarningSound != null)
        {
            m_AudioSource.clip = proximityWarningSound;
        }
        else
        {
            Debug.LogWarning("No proximity warning sound assigned to GhostProximityWarning on " + gameObject.name);
        }
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player transform not assigned and could not be found automatically!");
            }
        }
    }
    
    void Update()
    {
        if (player == null)
            return;
            
        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // If in warning distance and enough time has passed since last warning
        if (distanceToPlayer <= warningDistance && 
            Time.time - m_LastWarningTime > minTimeBetweenWarnings &&
            !m_HasPlayedWarning)
        {
            // Play warning sound
            if (m_AudioSource != null && m_AudioSource.clip != null && !m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
                m_LastWarningTime = Time.time;
                m_HasPlayedWarning = true;
            }
        }
        else if (distanceToPlayer > warningDistance && m_HasPlayedWarning)
        {
            // Reset the warning flag when player moves out of range
            m_HasPlayedWarning = false;
        }
    }
    
    // Visualize the warning range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, warningDistance);
    }
}