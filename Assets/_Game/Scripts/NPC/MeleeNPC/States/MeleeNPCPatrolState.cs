using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MeleeNPCPatrolState : NPCBaseState
{
    private readonly NPCSensor _sensor;
    private readonly NavMeshMove _navMeshMove;
    private int _patrolHash;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private List<Transform> _patrolPoints;
    private int _currentPatrolPoint;

    public MeleeNPCPatrolState(IStateSwitcher stateSwitcher,
        CharacterController characterController, NPCSensor sensor, Animator animator,
        NavMeshMove navMeshMove, Transform transform, List<Transform> patrolPoints)
        : base(stateSwitcher, characterController, animator,sensor, transform)
    {
        _sensor = sensor;
        _navMeshMove = navMeshMove;
        _patrolPoints = patrolPoints;
        _patrolHash = Animator.StringToHash("Walk");
    }

    public override void Enter()
    {
        animator.StopPlayback();
        animator.CrossFadeInFixedTime(_patrolHash, 0.1f);

        _currentPatrolPoint = 0;
        Patrol();
    }

    public override void Exit()
    {
        animator.StopPlayback();
        _disposable.Clear();
        _navMeshMove.StopMove();
    }

    private void Patrol()
    {
        _navMeshMove.MoveToPosition(_patrolPoints[_currentPatrolPoint].position);

        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (_navMeshMove != null && _navMeshMove.IsStopped())
            {
                if (_currentPatrolPoint < _patrolPoints.Count - 1)
                    _currentPatrolPoint++;
                else if (_currentPatrolPoint == _patrolPoints.Count - 1)
                    _currentPatrolPoint = 0;

                _navMeshMove.MoveToPosition(_patrolPoints[_currentPatrolPoint].position);
            }
        }).AddTo(_disposable);
    }
}