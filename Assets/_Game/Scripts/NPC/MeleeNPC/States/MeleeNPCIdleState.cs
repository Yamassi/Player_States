using UniRx;
using UnityEngine;

public class MeleeNPCIdleState : NPCBaseState
{
    private int _idleHash;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private NPCSensor _sensor;

    public MeleeNPCIdleState(IStateSwitcher stateSwitcher,
        CharacterController characterController, NPCSensor sensor, Animator animator, Transform transform)
        : base(stateSwitcher, characterController, animator, transform)
    {
        _sensor = sensor;
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
        }).AddTo(_disposable);
    }
}