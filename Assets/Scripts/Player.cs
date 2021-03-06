﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    ParticleSystem[] _explosions;

    [SerializeField]
    GameObject[]     _onOrOf;

    [SerializeField]
    Transform        _formationCollection;

    [Header("General")]
    [SerializeField]
    [Tooltip("In meters per second")]
    float       SPEED = 25.0f;

    [SerializeField]
    int         MAX_HEALTH = 50;

    [Header("Repairing")]
    [SerializeField]
    [Tooltip("In seconds")]
    [Range(1.0f, 10.0f)]
    float       TURRET_REPAIR_TIME = 5.0f;

    [SerializeField]
    [Tooltip("In seconds")]
    [Range(1.0f, 10.0f)]
    float       NPC_REPAIR_TIME = 5.0f;

    [SerializeField]
    [Tooltip("In seconds")]
    [Range(1.0f, 10.0f)]
    float       BARRIER_REPAIR_TIME = 2.5f;

    [SerializeField]
    [Tooltip("In seconds")]
    [Range(10.0f, 25.0f)]
    float       REACTOR_REPAIR_TIME = 15.0f;


    Transform   _transform;
    Rigidbody   _rigidbody;

    float       _targetTime;
    float       _repairTime;
    int         _health;


    void Start()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();

        _health = MAX_HEALTH;

        _repairTime = 0;
        _targetTime = NPC_REPAIR_TIME;

        _onOrOf[0].SetActive(false);
        _onOrOf[1].SetActive(true);
    }



    void Update()
    {
        if (_health > 0)
        {
            ProcessMovement();

            if (!CrossPlatformInputManager.GetButton("Fire1"))
            {
                _repairTime = 0;
            }
        }
    }

    private void ProcessMovement()
    {
        float xThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        float yThrow = CrossPlatformInputManager.GetAxis("Vertical");
        float throwSum = Mathf.Abs(xThrow) + Mathf.Abs(yThrow);

        if (throwSum >= 0.1f)
        {
            Vector3 velocity = new Vector3(xThrow, 0, yThrow);

            if (throwSum > 1.0f)
                velocity = velocity.normalized * SPEED;
            else
                velocity *= SPEED;

            _rigidbody.velocity = velocity;

            if (throwSum >= 0.25f)
            {
                Vector3 direction = velocity.normalized;
                float yRotation = Mathf.Atan2(-direction.z, direction.x) * 180 / Mathf.PI;

                _transform.rotation = Quaternion.Euler(0, yRotation, 0);
            }

            _formationCollection.transform.position = _transform.position;
        }
        else
            _rigidbody.velocity = Vector3.zero;
    }



    void OnParticleCollision(GameObject other)
    {
        _health--;

        if (_health <= 0)
            ProcessDeath();
    }

    private void ProcessDeath()
    {
        _onOrOf[0].SetActive(true);
        _onOrOf[1].SetActive(false);
        AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.EXPLOSION);

        Invoke("RestartLevel", 5.0f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }


    void OnCollisionStay(Collision collision)
    {
        //print("Colliding with: " + collision.collider.name);
        switch (collision.collider.tag)
        {
            case "Turret":
                ProcessCollisionWithTurret(collision);
                break;

            case "Reactor":
                ProcessCollisionWithReactor(collision);
                break;

            case "NPC":
                ProcessCollisionWithNPC(collision);
                break;

            case "Barrier":
                ProcessCollisionWithBarrier(collision);
                break;

            default:
                break;
        }
    }

    private void ProcessCollisionWithTurret(Collision collision)
    {
        if (_repairTime >= TURRET_REPAIR_TIME)
        {
            collision.gameObject.GetComponent<Turret>().Repaired();
            AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.REPAIR_COMPLETE);
        }

        _targetTime = TURRET_REPAIR_TIME;
        ProcessRepair();
    }

    private void ProcessCollisionWithReactor(Collision collision)
    {
        if (_repairTime >= REACTOR_REPAIR_TIME)
        {
            collision.gameObject.GetComponent<Reactor>().Repaired();
            AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.REPAIR_COMPLETE);
        }

        _targetTime = REACTOR_REPAIR_TIME;
        ProcessRepair();
    }

    private void ProcessCollisionWithNPC(Collision collision)
    {
        if (_repairTime >= NPC_REPAIR_TIME)
        {
            collision.gameObject.GetComponent<NPC>().Repaired();
            AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.REPAIR_COMPLETE);
        }

        _targetTime = NPC_REPAIR_TIME;
        ProcessRepair();
    }

    private void ProcessCollisionWithBarrier(Collision collision)
    {
        if (_repairTime >= BARRIER_REPAIR_TIME)
        {
            collision.gameObject.GetComponent<Barrier>().Repaired();
            AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.REPAIR_COMPLETE);
        }

        _targetTime = BARRIER_REPAIR_TIME;
        ProcessRepair();
    }


    private void ProcessRepair()
    {
        if (CrossPlatformInputManager.GetButton("Fire1"))
        {
            _repairTime += Time.deltaTime;

            if (_repairTime == 0.0f)
                AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.REPAIRING);
        }
        else
            _repairTime = 0.0f;
    }



    public float RepairFraction
    {
        get { return _repairTime / _targetTime; }
    }

    public float HPFraction
    {
        get { return _health / (float)MAX_HEALTH; }
    }
}
