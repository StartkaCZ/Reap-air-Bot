using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    [Tooltip("In meters per second")]
    float   SPEED = 25.0f;

    [SerializeField]
    int     MAX_HEALTH = 50;

    int     _health;


    void Start()
    {
        _health = MAX_HEALTH;
    }



    void Update()
    {

    }


    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
            ProcessDeath();
    }

    private void ProcessDeath()
    {
        // explode
    }


    void OnParticleCollision(GameObject other)
    {
        TakeDamage(1);
    }
}
