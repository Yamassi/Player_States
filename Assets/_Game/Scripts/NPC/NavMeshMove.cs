using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMove : MonoBehaviour
{
    [SerializeField] private float _displacementDist = 3f;
    private NavMeshAgent _agent;

    private float _randomAngle,
        _randomAngleCoolDown = 5f,
        _randomAngleTimer,
        _randomPositionTimer,
        _randomPositionCoolDown,
        _walkSpeed = 1,
        _runSpeed = 3,
        _currentWalkSpeed,
        _currentRunSpeed;

    private bool _isFirstChase = true, _isStopped = true, _isResetRandomPosition = true, _isWalk = true;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        ResetSpeed();
    }

    public void ResetSpeed()
    {
        _currentWalkSpeed = _walkSpeed;
        _currentRunSpeed = _runSpeed;
    }

    public void SetSpeed(float divideSpeed)
    {
        _currentRunSpeed = _runSpeed / divideSpeed;
        _currentWalkSpeed = _walkSpeed / divideSpeed;

        if (!_isWalk)
        {
            _agent.speed = _currentRunSpeed;
        }
        else
        {
            _agent.speed = _currentWalkSpeed;
        }
    }

    public void StopMove()
    {
        _agent.ResetPath();
    }

    public bool IsStopped()
    {
        if (_agent.remainingDistance > 0)
            return _isStopped = false;
        else
            return _isStopped = true;
    }

    public void Walk()
    {
        if (IsStopped())
        {
            GetPosition();
        }
    }

    public void Chase(Transform targetPosition)
    {
        _isWalk = false;
        if (targetPosition == null)
        {
            _isFirstChase = true;
            return;
        }

        MoveToPosition(targetPosition.position);
    }

    public void RunAway(Transform chaserPosition)
    {
        _isWalk = false;
        if (chaserPosition == null)
        {
            _randomAngleTimer = 0;
            _isFirstChase = true;
            return;
        }

        GetAngle();

        Vector3 normDirection = (chaserPosition.position - transform.position).normalized;
        normDirection = Quaternion.AngleAxis(_randomAngle, Vector3.up) * normDirection;
        MoveToPosition(transform.position - (normDirection * _displacementDist));
    }

    private void GetAngle()
    {
        if (_isFirstChase)
        {
            GetRandomAngle();

            _isFirstChase = false;
        }
        else
        {
            if (_randomAngleTimer > _randomAngleCoolDown)
            {
                GetRandomAngle();
                _randomAngleTimer = 0;
            }
        }

        _randomAngleTimer += Time.deltaTime;
    }

    private void GetRandomAngle()
    {
        _randomAngle = Random.Range(-89, 91);
    }

    private void GetPosition()
    {
        if (_isResetRandomPosition)
        {
            _randomPositionCoolDown = Random.Range(1, 5);
            MoveToPosition(GetRandomPosition());
            _isResetRandomPosition = false;
            _isWalk = true;
        }
        else
        {
            if (_randomPositionTimer > _randomPositionCoolDown)
            {
                _isResetRandomPosition = true;
                _randomPositionTimer = 0;
            }
        }

        _randomPositionTimer += Time.deltaTime;
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(transform.position.x + (Random.Range(-1f, 1f)), 0,
            transform.position.z + (Random.Range(-1f, 1f)));
    }

    public void MoveToPosition(Vector3 position)
    {
        _agent.SetDestination(position);
        _agent.isStopped = false;
    }

    public void RunToggle(bool isRun)
    {
        if (isRun)
        {
            _agent.speed = _currentRunSpeed;
            _agent.angularSpeed = 1200;
            _agent.acceleration = 8;
        }
        else
        {
            _agent.speed = _currentWalkSpeed;
            _agent.angularSpeed = 700;
            _agent.acceleration = 3;
        }
    }

    public bool IsWalk()
    {
        return _isWalk;
    }
}