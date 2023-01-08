using UnityEngine;
using UniRx;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private ReactiveProperty<EnemyAnimationState> _currentState = new ReactiveProperty<EnemyAnimationState>(EnemyAnimationState.Walking);

    private void Awake()
    {
        _currentState
            .Subscribe(state => OnStateChanged(state))
            .AddTo(this);
    }

    public void ChangeStateTo(EnemyAnimationState newState)
    {
        _currentState.Value = newState;
    }

    public void PlayTakeDamage()
    {
        _animator.SetTrigger(EnemyAnimationParams.TakeDamage);
    }

    public void PlayAttack()
    {
        _animator.SetTrigger(EnemyAnimationParams.StabAttack);
    }

    private void OnStateChanged(EnemyAnimationState state)
    {
        switch (state)
        {
            case EnemyAnimationState.Walking:
                _animator.SetBool(EnemyAnimationParams.WalkForward, true);
                break;
            case EnemyAnimationState.Idle:
                _animator.SetBool(EnemyAnimationParams.WalkForward, false);
                break;
            case EnemyAnimationState.Dead:
                _animator.SetBool(EnemyAnimationParams.WalkForward, false);
                _animator.SetTrigger(EnemyAnimationParams.Die);
                break;
            default:
                break;
        }
    }
}
