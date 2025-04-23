using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    public float sprintMultiplier = 2f; // Speed multiplier when sprinting
    public float sprintDuration = 2f;  // Duration of the sprint in seconds
    public float sprintCooldown = 5f;  // Cooldown time before sprinting again
    public Image SprintIcon;           // Reference to the SprintIcon UI element

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    float sprintTimer = 0f;
    float cooldownTimer = 0f;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        sprintTimer = sprintDuration; // Initialize sprint timer to full duration
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        // Sprint logic
        if (Input.GetKey(KeyCode.LeftShift) && cooldownTimer <= 0f)
        {
            if (sprintTimer > 0f)
            {
                m_Movement *= sprintMultiplier;
                sprintTimer -= Time.deltaTime;

                // Update SprintIcon fill amount based on sprint timer
                if (SprintIcon != null)
                {
                    SprintIcon.fillAmount = sprintTimer / sprintDuration;
                }
            }
            else
            {
                cooldownTimer = sprintCooldown;
            }
        }
        else
        {
            if (sprintTimer < sprintDuration && cooldownTimer <= 0f)
            {
                sprintTimer += Time.deltaTime; // Recharge sprint timer when not sprinting
            }

            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }

            // Reset SprintIcon fill amount to max when not sprinting
            if (SprintIcon != null)
            {
                SprintIcon.fillAmount = sprintTimer / sprintDuration;
            }
        }

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

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0.0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}
