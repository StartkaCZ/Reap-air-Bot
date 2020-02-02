using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationPoint : MonoBehaviour
{
    Transform   _transform;

    bool        _occupied;


    void Awake()
    {
        _transform = transform;

        _occupied = false;
    }


    public bool occupied
    {
        get { return _occupied; }
        set { _occupied = value; }
    }

    public Vector3 position
    {
        get { return _transform.position; }
    }
}
