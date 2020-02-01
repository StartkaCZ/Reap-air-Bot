using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [SerializeField]
    [Tooltip("In meters per second")]
    float               SPEED = 25.0f;

    [SerializeField]
    int                 MAX_HEALTH = 100;

    
    Transform           _goal;
    NavMeshAgent        _agent;

    int                 _health;



    void Start()
    {
        _health = MAX_HEALTH;

        _goal = GameObject.FindObjectOfType<Player>().transform;

        _agent = GetComponent<NavMeshAgent>();
        _agent.destination = _goal.position;
    }



    void Update()
    {

    }


    public void Repaired()
    {
        // change mesh
        _health = MAX_HEALTH;
    }



    void OnParticleCollision(GameObject other)
    {

    }
}
