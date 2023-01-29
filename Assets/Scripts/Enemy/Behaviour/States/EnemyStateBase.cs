using UniRx;
using UnityEngine;

public abstract class EnemyStateBase : IEnemyState
{
    protected IEnemyStateSwitcher StateSwitcher;
    protected ITargetProvider TargetProvider;
    protected EnemyAnimator Animator;

    protected CompositeDisposable _disposables = new();

    public EnemyStateBase(
        IEnemyStateSwitcher stateSwitcher, 
        ITargetProvider targetProvider,
        EnemyAnimator enemyAnimator)
    {
        StateSwitcher = stateSwitcher;
        TargetProvider = targetProvider;
        Animator = enemyAnimator;
    }

    public virtual void OnStateEnter() { }

    public virtual void OnStateExit() 
    {
        _disposables.Clear();
    }

    public virtual void OnStateDestroyed()
    {
        _disposables.Dispose();
    }

    public virtual void OnTriggerEnter(Collider other) { }

    public virtual void OnTriggerExit(Collider other) { }

    protected void SwitchState<T>() where T : IEnemyState
    {
        StateSwitcher.SwitchStateTo<T>();
    }

    protected DamageTarget GetTarget()
    {
        return TargetProvider.GetTarget();
    }

    protected void SetTarget(DamageTarget target)
    {
        TargetProvider.SetTarget(target);
    }
}
