using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int _punchLHash, _punchRHash;
    private bool _isFirstPunch = true, _isAnimationEnd = false;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private CancellationTokenSource _cts;

    public PlayerAttackState(IStateSwitcher stateSwitcher, PlayerInput playerInput,
        ControlInput controlInput, CharacterController characterController,
        Animator animator, Transform transform) : base(stateSwitcher, playerInput, controlInput,
        characterController, animator, transform)
    {
        _punchLHash = Animator.StringToHash("PunchL");
        _punchRHash = Animator.StringToHash("PunchR");
    }

    public override void Enter()
    {
        _cts = new();
        Debug.Log("Attack");
        animator.StopPlayback();
        Attack();
    }

    public override void Exit()
    {
        _disposable.Clear();
        _cts.Cancel();
        _cts.Dispose();
        animator.StopPlayback();
    }

    private async void Attack()
    {
        if (Player.isUnarmed)
        {
            if (_isFirstPunch)
                animator.CrossFadeInFixedTime(_punchLHash, 0.1f);
            if (!_isFirstPunch)
                animator.CrossFadeInFixedTime(_punchRHash, 0.1f);

            _isFirstPunch = !_isFirstPunch;
        }
        await UniTask.Delay(500, cancellationToken: _cts.Token);
        Player.isRequiredNewAttackPress = true;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (!controlInput.IsRunPressed)
                HandleInput(_walkSpeed);
            if (controlInput.IsRunPressed)
                HandleInput(_runSpeed);
            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                 if (controlInput.IsRunPressed && controlInput.CurrentMovementInput.x != 0
                         || controlInput.CurrentMovementInput.y != 0)
                    stateSwitcher.SwitchState<PlayerRunState>();
                 else  if (!controlInput.IsRunPressed && controlInput.CurrentMovementInput.x != 0
                           || controlInput.CurrentMovementInput.y != 0)
                     stateSwitcher.SwitchState<PlayerWalkState>();
                 else if (controlInput.CurrentMovementInput is { x: 0, y: 0 })
                    stateSwitcher.SwitchState<PlayerIdleState>();

                
            }
        }).AddTo(_disposable);
    }
}