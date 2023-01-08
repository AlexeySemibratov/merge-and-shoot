using UniRx;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private ReactiveProperty<PlayerAnimationState> _currentState = new ReactiveProperty<PlayerAnimationState>(PlayerAnimationState.Idle);

    private void Awake()
    {
        _currentState.
            ObserveOnMainThread()
            .Subscribe(state => OnAnimationStateChanged(state))
            .AddTo(this);
    }

    public void ChangeAnimationStateTo(PlayerAnimationState newState)
    {
        _currentState.Value = newState;
    }

    public void PlayShoot()
    {
        _animator.SetTrigger(PlayerAnimationParams.SingleShoot);
    }

    private void OnAnimationStateChanged(PlayerAnimationState newState)
    {
        switch (newState)
        {
            case PlayerAnimationState.Idle:
                _animator.SetBool(PlayerAnimationParams.Walk, false);
                break;
            case PlayerAnimationState.Walk:
                _animator.SetBool(PlayerAnimationParams.Walk, true);
                break;
            case PlayerAnimationState.Dead:
                _animator.SetTrigger(PlayerAnimationParams.Die);
                break;
            default:
                break;
        }
    }
}
