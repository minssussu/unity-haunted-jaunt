using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    //for the gambling beds
    int rand_bed_action = -1;
    GameObject last_bed;
    Vector3 rise_to_pos;
    private float time_spent_rising;
    private float time_to_spend_rising = 3;

    public ParticleSystem particle_system;
    bool isWalking;

    Vector3 original_player_pos;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();
        //particle_system = GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {
        if (rand_bed_action >= 0)
        {
            if (time_spent_rising >= time_to_spend_rising)
            {
                this.transform.position = new Vector3(-9.8f, 0.0f, -3.2f);
                last_bed.SetActive(true);
                rand_bed_action = -1;
                particle_system.Stop();
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
            isWalking = hasHorizontalInput || hasVerticalInput;
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
                //send player to the end
                rand_bed_action = -1;
                this.transform.position = new Vector3(18, 0, 2);
            }
            else
            {
                last_bed = other.gameObject;
                last_bed.SetActive(false);
                original_player_pos = this.transform.position;
                rise_to_pos = original_player_pos;
                rise_to_pos.y += 10;
                time_spent_rising = 0;
                particle_system.Play();
                isWalking = false;
                m_AudioSource.Stop();
            }
        }
    }
}