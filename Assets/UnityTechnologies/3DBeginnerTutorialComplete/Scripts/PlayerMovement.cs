using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    // Using existing sound files from the project
    private AudioClip m_TeleportSuccessSound; // SFXWin.wav will be used
    private AudioClip m_TeleportFailSound;   // SFXGameOver.wav will be used

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    AudioSource m_BedAudioSource; // Separate audio source for bed sound effects
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    //for the gambling beds
    int rand_bed_action = -1;
    GameObject last_bed;
    Vector3 rise_to_pos;
    private float time_spent_rising;
    private float time_to_spend_rising = 3;

    Vector3 original_player_pos;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();
        
        // Create a separate audio source for bed sound effects
        m_BedAudioSource = gameObject.AddComponent<AudioSource>();
        m_BedAudioSource.playOnAwake = false;
        m_BedAudioSource.spatialBlend = 0.0f; // Make it 2D sound (follows player)
        m_BedAudioSource.volume = 1.0f;
        
        // Load existing sound effects from the project
        m_TeleportSuccessSound = Resources.Load<AudioClip>("UnityTechnologies/3DBeginnerTutorialComplete/Audio/SFXWin");
        m_TeleportFailSound = Resources.Load<AudioClip>("UnityTechnologies/3DBeginnerTutorialComplete/Audio/SFXGameOver");
        
        // If we couldn't load them from Resources, try to find them directly
        if (m_TeleportSuccessSound == null)
        {
            AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource source in audioSources)
            {
                if (source.clip != null)
                {
                    if (source.clip.name == "SFXWin")
                        m_TeleportSuccessSound = source.clip;
                    else if (source.clip.name == "SFXGameOver")
                        m_TeleportFailSound = source.clip;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (rand_bed_action >= 0)
        {
            if (time_spent_rising >= time_to_spend_rising)
            {
                // Play a teleport sound when the player is teleported back to start
                if (m_TeleportFailSound != null)
                {
                    m_BedAudioSource.clip = m_TeleportFailSound;
                    m_BedAudioSource.Play();
                }
                
                this.transform.position = new Vector3(-9.8f, 0.0f, -3.2f);
                last_bed.SetActive(true);
                rand_bed_action = -1;
            }
            else
            {
                //add time since last frame to the timer
                time_spent_rising += Time.deltaTime;
                //set player position to position in the air. resembles the "my people need me" meme
                this.transform.position = Vector3.Lerp(original_player_pos, rise_to_pos, time_spent_rising / time_to_spend_rising);
            }
        }
        else
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            m_Movement.Set(horizontal, 0f, vertical);
            m_Movement.Normalize();

            bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
            bool isWalking = hasHorizontalInput || hasVerticalInput;
            m_Animator.SetBool("IsWalking", isWalking);

            if (isWalking)
            {
                if (!m_AudioSource.isPlaying)
                {
                    m_AudioSource.Play();
                }
            }
            else
            {
                m_AudioSource.Stop();
            }

            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
        }
    }
    void OnAnimatorMove ()
    {
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation (m_Rotation);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bed"))
        {
            //1 in 10 chance of being sent to the end, otherwise rise in the air and teleport to the start
            // < insert "my people need me" meme here >
            rand_bed_action = Random.Range(0, 11);

            if (rand_bed_action == 10)
            {
                // Play the success teleport sound
                if (m_TeleportSuccessSound != null)
                {
                    m_BedAudioSource.clip = m_TeleportSuccessSound;
                    m_BedAudioSource.Play();
                }
                
                //send player to the end
                rand_bed_action = -1;
                this.transform.position = new Vector3(18, 0, 2);
            }
            else
            {
                // Play the failure/rise sound
                if (m_TeleportFailSound != null)
                {
                    m_BedAudioSource.clip = m_TeleportFailSound;
                    m_BedAudioSource.Play();
                }
                
                last_bed = other.gameObject;
                last_bed.SetActive(false);
                original_player_pos = this.transform.position;
                rise_to_pos = original_player_pos;
                rise_to_pos.y += 10;
                time_spent_rising = 0;
            }
        }
    }
}