using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class NPC : MonoBehaviour, IStateSwitcher
{
    protected CharacterController _characterController;
    protected Animator _animator;

    protected StateMachine _stateMachine;
    protected List<BaseState> _allStates;

    protected virtual void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    public void SwitchState<T>() where T : BaseState
    {
        var state = _allStates.FirstOrDefault(s => s is T);
        _stateMachine.ChangeState(state);
    }
}