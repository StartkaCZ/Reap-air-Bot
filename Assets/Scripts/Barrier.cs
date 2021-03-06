﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField]
    ParticleSystem[]    _explosions;

    [SerializeField]
    GameObject[]        _onOrOf;

    [SerializeField]
    int                 MAX_HEALTH = 25;
    int                 _enemiesToWithstand;

    [SerializeField]
    int                 DAMAGE = 2;


    void Start()
    {
        _onOrOf[0].SetActive(true);
        _onOrOf[1].SetActive(false);

        foreach (ParticleSystem explosion in _explosions)
        {
            ParticleSystem.EmissionModule emissionModule = explosion.emission;
            emissionModule.enabled = true;
        }
    }


    public void Repaired()
    {
        _enemiesToWithstand = MAX_HEALTH;

        _onOrOf[0].SetActive(false);
        _onOrOf[1].SetActive(true);

        foreach (ParticleSystem explosion in _explosions)
        {
            ParticleSystem.EmissionModule emissionModule = explosion.emission;
            emissionModule.enabled = false;
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            _enemiesToWithstand--;

            if (_enemiesToWithstand <= 0)
                ProcessDeath();

            other.GetComponent<Enemy>().TakeDamage(DAMAGE);
        }
    }

    private void ProcessDeath()
    {
        AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.EXPLOSION);

        _onOrOf[0].SetActive(true);
        _onOrOf[1].SetActive(false);

        foreach (ParticleSystem explosion in _explosions)
        {
            ParticleSystem.EmissionModule emissionModule = explosion.emission;
            emissionModule.enabled = true;
        }
    }
}
