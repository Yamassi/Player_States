using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MeleeNPCIdleState : NPCBaseState
{
    private readonly NPCSensor _sensor;
    private int _idleHash;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private List<Transform> _patrolPoints;

    public MeleeNPCIdleState(IStateSwitcher stateSwitcher,
        CharacterController characterController, NPCSensor sensor, Animator animator,
        Transform transform, List<Transform> patrolPoints)
        : base(stateSwitcher, characterController, animator, sensor,transform)
    {
        _sensor = sensor;
        _patrolPoints = patrolPoints;
        _idleHash = Animator.StringToHash("Idle");
    }

    public override void Enter()
    {
        animator.StopPlayback();
        animator.CrossFadeInFixedTime(_idleHash, 0.1f);
        Idle();
    }

    public override void Exit()
    {
        animator.StopPlayback();
        _disposable.Clear();
    }

    private void Idle()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (_sensor.IsHitTaking)
                stateSwitcher.SwitchState<MeleeNPCTakeHitState>();
            if (_patrolPoints.Count > 1)
                stateSwitcher.SwitchState<MeleeNPCPatrolState>();
        }).AddTo(_disposable);
    }
}