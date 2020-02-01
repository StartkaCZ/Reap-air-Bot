using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : MonoBehaviour
{
    [SerializeField]
    int     MAX_HEALTH = 100;

    int     _health;


    void Start()
    {
        _health = MAX_HEALTH;
    }



    void Update()
    {

    }


    public void Repaired()
    {
        // change mesh
        _health = MAX_HEALTH;
        // take into account REACTOR_ENERGY
    }



    private void OnTriggerEnter(Collider other)
    {
        _health--;

        if (_health <= 0)
            ProcessDeath();
    }

    private void ProcessDeath()
    {
        // switch model
        // disable particles
    }
}
