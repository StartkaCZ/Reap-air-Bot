using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [SerializeField]
    [Tooltip("In meters per second")]
    float               SPEED = 25.0f;

    [SerializeField]
    [Range(5.0f, 12.5f)]
    float               LINE_OF_SIGHT = 10.0f;

    [SerializeField]
    int                 MAX_HEALTH = 100;

    
    Transform           _target;
    Transform           _transform;

    Rigidbody           _rigidbody;
    NavMeshAgent        _agent;
    FormationPoint      _formationPoint;

    int                 _health;



    void Start()
    {
        _health = 0;

        FormationPoint[] formationPoints = GameObject.FindObjectsOfType<FormationPoint>();

        for (int i = 0; i < formationPoints.Length; i++)
        {
            if (!formationPoints[i].occupied)
            {
                _formationPoint = formationPoints[i];
                _formationPoint.occupied = true;
                break;
            }
        }

        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;

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
            _agent.destination = _formationPoint.position;

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
    }


    public void Repaired()
    {
        // change mesh
        _health = MAX_HEALTH;
        _rigidbody.isKinematic = false;
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
        _formationPoint.occupied = false;
        _rigidbody.isKinematic = true;
        // explode
        // change mesh
    }
}
