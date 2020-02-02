using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField]
    [Range(2.5f, 7.5f)]
    float       LINE_OF_SIGHT = 5.0f;

    [SerializeField]
    int         MAX_HEALTH = 100;

    Transform   _transform;
    Transform   _target;

    int         _health;


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
            }
            else
            {
                _transform.LookAt(_target, Vector3.up);
                // FIRE
            }
        }
    }


    public void Repaired()
    {
        // change mesh
        _health = 100;
    }



    void OnTriggerStay(Collider other)
    {
        if (_health > 0)
        {
            if (_target == null && other.tag == "Enemy")
            {
                _target = other.transform;
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
        // switch model
    }
}
