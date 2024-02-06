using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeNPC : NPC
{
    private NPCSensor _sensor;

    protected override void Awake()
    {
        base.Awake();
        _stateMachine = new();
        _sensor = new();

        _allStates = new()
        {
            new MeleeNPCIdleState(this, _characterController, _sensor, _animator, transform),
            new MeleeNPCTakeHitState(this, _characterController, _sensor, _animator, transform)
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