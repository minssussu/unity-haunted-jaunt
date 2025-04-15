using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundEffect : MonoBehaviour
{
    public enum TriggerType
    {
        OnProximity,     // Triggers when player gets close
        OnTriggerEnter,  // Triggers when player enters trigger collider
        OnTriggerStay,   // Triggers while player stays in trigger collider
        RandomInterval   // Triggers at random intervals
    }
    
    public TriggerType triggerType = TriggerType.OnProximity;
    public float proximityDistance = 3.0f;
    public float minInterval = 5.0f;
    public float maxInterval = 15.0f;
    public AudioClip[] soundEffects;
    public float volume = 1.0f;
    public bool randomizePitch = true;
    public float pitchRange = 0.2f;
    
    private AudioSource m_AudioSource;
    private Transform m_Player;
    private float m_NextPlayTime;
    
    void Start()
    {
        // Find the player if not specified
        m_Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Setup audio source
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        m_AudioSource.playOnAwake = false;
        m_AudioSource.spatialBlend = 1.0f; // 3D sound by default
        m_AudioSource.volume = volume;
        
        // Set initial time for random interval sounds
        if (triggerType == TriggerType.RandomInterval)
        {
            ScheduleNextRandomPlay();
        }
    }
    
    void Update()
    {
        if (m_Player == null || soundEffects == null || soundEffects.Length == 0)
            return;
            
        switch (triggerType)
        {
            case TriggerType.OnProximity:
                CheckProximity();
                break;
                
            case TriggerType.RandomInterval:
                CheckRandomInterval();
                break;
        }
    }
    
    void CheckProximity()
    {
        float distance = Vector3.Distance(transform.position, m_Player.position);
        
        if (distance <= proximityDistance && !m_AudioSource.isPlaying && Time.time >= m_NextPlayTime)
        {
            PlayRandomSound();
            m_NextPlayTime = Time.time + minInterval;
        }
    }
    
    void CheckRandomInterval()
    {
        if (Time.time >= m_NextPlayTime)
        {
            PlayRandomSound();
            ScheduleNextRandomPlay();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.OnTriggerEnter && other.CompareTag("Player") && Time.time >= m_NextPlayTime)
        {
            PlayRandomSound();
            m_NextPlayTime = Time.time + minInterval;
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (triggerType == TriggerType.OnTriggerStay && other.CompareTag("Player") && !m_AudioSource.isPlaying && Time.time >= m_NextPlayTime)
        {
            PlayRandomSound();
            m_NextPlayTime = Time.time + minInterval;
        }
    }
    
    void PlayRandomSound()
    {
        // Select a random sound from the array
        int soundIndex = Random.Range(0, soundEffects.Length);
        m_AudioSource.clip = soundEffects[soundIndex];
        
        // Randomize pitch if enabled
        if (randomizePitch)
        {
            m_AudioSource.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        }
        else
        {
            m_AudioSource.pitch = 1.0f;
        }
        
        m_AudioSource.Play();
    }
    
    void ScheduleNextRandomPlay()
    {
        m_NextPlayTime = Time.time + Random.Range(minInterval, maxInterval);
    }
    
    // Visualize the proximity range in the editor when proximity trigger is selected
    void OnDrawGizmosSelected()
    {
        if (triggerType == TriggerType.OnProximity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, proximityDistance);
        }
    }
}