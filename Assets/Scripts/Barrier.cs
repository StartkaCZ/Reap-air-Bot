using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField]
    int     MAX_HEALTH = 25;
    int     _enemiesToWithstand;

    [SerializeField]
    int     DAMAGE = 2;


    public void Repaired()
    {
        // change mesh
        _enemiesToWithstand = MAX_HEALTH;
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
        // switch model
        // disable particles
    }
}
