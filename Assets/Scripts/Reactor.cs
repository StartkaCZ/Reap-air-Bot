using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : MonoBehaviour
{
    [SerializeField]
    GameObject[]        _onOrOf;

    [SerializeField]
    int     MAX_HEALTH = 100;

    int     _health;


    void Start()
    {
        _health = MAX_HEALTH;

        _onOrOf[0].SetActive(false);
        _onOrOf[1].SetActive(true);
    }



    void Update()
    {

    }


    public void Repaired()
    {
        _onOrOf[0].SetActive(false);
        _onOrOf[1].SetActive(true);

        _health = MAX_HEALTH;
        // take into account REACTOR_ENERGY
        AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.EXPLOSION);
    }



    private void OnTriggerEnter(Collider other)
    {
        _health--;

        if (_health <= 0)
            ProcessDeath();
    }

    private void ProcessDeath()
    {
        _onOrOf[0].SetActive(true);
        _onOrOf[1].SetActive(false);
        // disable particles
    }
}
