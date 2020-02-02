using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    ParticleSystem[]    _guns;

    [SerializeField]
    ParticleSystem[]    _explosions;

    [SerializeField]
    [Tooltip("In meters per second")]
    float               SPEED = 25.0f;

    [SerializeField]
    [Range(5.0f, 12.5f)]
    float               LINE_OF_SIGHT = 10.0f;

    [SerializeField]
    int                 MAX_HEALTH = 50;


    Transform           _player;
    Transform           _target;
    Transform           _transform;

    NavMeshAgent        _agent;

    int                 _health;


    void Start()
    {
        _health = MAX_HEALTH;

        _transform = transform;
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = SPEED;


        BoxCollider[] colliders = GetComponents<BoxCollider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            SetBoxCollider(colliders[i]);
        }
    }

    private void SetBoxCollider(BoxCollider collider)
    {
        if (collider.isTrigger)
        {
            Vector3 size = collider.size;

            collider.size = new Vector3(LINE_OF_SIGHT, size.y, LINE_OF_SIGHT);
        }
    }



    void Update()
    {
        if (_health > 0)
        {
            _agent.destination = _player.position;

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
                    _transform.LookAt(_target, Vector3.up);
                }
            }
        }
    }




    void OnParticleCollision(GameObject other)
    {
        TakeDamage(1);
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
        _target = null;
        AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.EXPLOSION);
        Destroy(gameObject);
    }



    void OnTriggerStay(Collider other)
    {
        if (_health > 0)
        {
            if (_target == null)
            {
                ProcessNewTarget(other);
            }
            else
            {
                PrioritiseTarget(other);
            }

            foreach (ParticleSystem gun in _guns)
            {
                ParticleSystem.EmissionModule emissionModule = gun.emission;
                emissionModule.enabled = true;
            }
        }
    }

    private void ProcessNewTarget(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Reactor" ||
                    other.tag == "NPC" || other.tag == "Turret")
            _target = other.transform;
    }

    private void PrioritiseTarget(Collider other)
    {
        string currentTarget = _target.tag;

        if (currentTarget == "Turret")
        {
            if (other.tag == "Player")
                _target = other.transform;
            else if (other.tag == "Reactor")
                _target = other.transform;
            else if (other.tag == "NPC")
                _target = other.transform;
        }
        if (currentTarget == "NPC")
        {
            if (other.tag == "Player")
                _target = other.transform;
            else if (other.tag == "Reactor")
                _target = other.transform;
        }
        if (currentTarget == "Reactor")
        {
            if (other.tag == "Player")
                _target = other.transform;
        }
    }
}
