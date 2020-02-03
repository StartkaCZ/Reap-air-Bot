using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField]
    ParticleSystem[]    _guns;

    [SerializeField]
    ParticleSystem[]    _explosions;

    [SerializeField]
    GameObject[]        _onOrOf;

    [SerializeField]
    Transform           _head;

    [SerializeField]
    Transform           _base;

    [SerializeField]
    [Range(2.5f, 10.0f)]
    float               LINE_OF_SIGHT = 10.0f;

    [SerializeField]
    int                 MAX_HEALTH = 100;

    Transform           _transform;
    Transform           _target;

    int                 _health;


    void Start()
    {
        BoxCollider[] colliders = GetComponents<BoxCollider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            SetBoxCollider(colliders[i]);
        }

        _transform = transform;
        for (int i = 0; i < _transform.childCount; i++)
        {
            if (_transform.GetChild(i).name == "Head")
            {
                _transform = _transform.GetChild(i);
                break;
            }
        }

        _health = 0;

        _onOrOf[0].SetActive(true);
        _onOrOf[1].SetActive(false);

        foreach (ParticleSystem explosion in _explosions)
        {
            ParticleSystem.EmissionModule emissionModule = explosion.emission;
            emissionModule.enabled = true;
        }
    }

    private void SetBoxCollider(BoxCollider collider)
    {
        if (collider.isTrigger)
        {
            Vector3 size = collider.size;

            collider.size = new Vector3(LINE_OF_SIGHT * ConstantHolder.GRID_SIZE, 
                                        size.y, 
                                        LINE_OF_SIGHT * ConstantHolder.GRID_SIZE);
        }
    }



    void Update()
    {
        if (_health > 0)
        {
            ProcessTarget();
        }
    }

    private void ProcessTarget()
    {
        if (_target != null)
        {
            float distanceFromTarget = Vector3.Distance(_transform.position, _target.position);
            float maxDistance = LINE_OF_SIGHT * ConstantHolder.GRID_SIZE * 0.5f;

            if (distanceFromTarget > maxDistance)
            {
                _target = null;

                foreach (ParticleSystem gun in _guns)
                {
                    ParticleSystem.EmissionModule emissionModule = gun.emission;
                    emissionModule.enabled = false;
                }
            }
            else
            {
                Vector3 direction = _target.position - _transform.position;

                _base.rotation = Quaternion.Euler(0, Mathf.Atan2(-direction.z, direction.x) * 180/Mathf.PI, 0);
                _head.LookAt(_target, Vector3.up);
            }
        }
    }


    public void Repaired()
    {
        _onOrOf[0].SetActive(false);
        _onOrOf[1].SetActive(true);
        _health = MAX_HEALTH;

        foreach (ParticleSystem explosion in _explosions)
        {
            ParticleSystem.EmissionModule emissionModule = explosion.emission;
            emissionModule.enabled = false;
        }
    }



    void OnTriggerStay(Collider other)
    {
        if (_health > 0)
        {
            if (_target == null && other.tag == "Enemy")
            {
                _target = other.transform;

                foreach (ParticleSystem gun in _guns)
                {
                    ParticleSystem.EmissionModule emissionModule = gun.emission;
                    emissionModule.enabled = true;
                }
            }
        }
    }


    void OnParticleCollision(GameObject other)
    {
        _health--;

        if (_health <= 0)
            ProcessDeath();
    }

    private void ProcessDeath()
    {
        _target = null;
        AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.EXPLOSION);
        _onOrOf[0].SetActive(true);
        _onOrOf[1].SetActive(false);

        foreach (ParticleSystem explosion in _explosions)
        {
            ParticleSystem.EmissionModule emissionModule = explosion.emission;
            emissionModule.enabled = true;
        }

        foreach (ParticleSystem gun in _guns)
        {
            ParticleSystem.EmissionModule emissionModule = gun.emission;
            emissionModule.enabled = false;
        }
    }
}
