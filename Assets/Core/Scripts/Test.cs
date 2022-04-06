using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    private Animator _anim;
    private NavMeshAgent _navAgent;
    private Rigidbody _rigidbody;
    private List<Vector3> _path = new List<Vector3>();
    private int _pathIndex;
    private bool _pathReady = false;
    private Transform _playerTransform;
    private AlignToAngle _angleToPlater;
    private string _rotIndex = "RotationIndex";
    private Vector3 _targetPos;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        _angleToPlater = GetComponent<AlignToAngle>();
    }

    private void Start()
    {
        _pathIndex = 0;
        for (int i = 0; i < 7; i++)
        {
            _path.Add(GetRandomPosition());
        }
        _pathReady = true;
        _playerTransform = PlayerController.Instance.transform;
    }

    private void Update()
    { 
        _angleToPlater.CalculateAngle(_targetPos);
        if (_angleToPlater.ReadyForAnim)
        {
            _anim.SetFloat(_rotIndex, _angleToPlater.LastIndex);
        }
        if (_pathReady)
        {
            FollowPath();
        }

        //Vector3 _targetPosition = _playerTransform.position;
        //_targetPosition.y = transform.position.y;
        //transform.LookAt(_targetPosition);        
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        return transform.position + randomDir * UnityEngine.Random.Range(50f, 100f);
    }

    private void FollowPath()
    {
        if (_navAgent.remainingDistance <= 5f)
        {
            _pathIndex++;
            if (_pathIndex >= _path.Count)
            {
                _pathIndex = 0;
            }
            _navAgent.SetDestination(_path[_pathIndex]);
            _targetPos = new Vector3(_navAgent.destination.x, transform.position.y, _navAgent.destination.z);
        }       
    }



}
