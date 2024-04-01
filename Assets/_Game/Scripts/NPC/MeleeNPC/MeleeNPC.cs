using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeNPC : NPC
{
    [SerializeField] private List<Transform> patrolPoints;
    private NPCSensor _sensor;

    protected override void Awake()
    {
        base.Awake();
        _stateMachine = new();
        _sensor = new();

        _allStates = new()
        {
            new MeleeNPCIdleState(this, _characterController, _sensor, _animator, transform, patrolPoints),
            new MeleeNPCTakeHitState(this, _characterController, _sensor, _animator, transform),
            new MeleeNPCPatrolState(this, _characterController, _sensor, _animator, _navMeshMove, transform,
                patrolPoints)
        };

        _stateMachine.Init(_allStates[0]);

        _sensor.IsHitTaking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Hand>())
            _sensor.IsHitTaking = true;
    }
}

public class NPCSensor
{
    public bool IsHitTaking;
}