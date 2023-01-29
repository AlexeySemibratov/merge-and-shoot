using System;
using UniRx;
using UnityEngine;

public class AttackTarget : EnemyStateBase
{
    private IDamageTarget _attacker;
    private float _attackDelay;
    private int _damagePerAttack;

    public AttackTarget(
        IEnemyStateSwitcher stateSwitcher,
        ITargetProvider targetProvider,
        EnemyAnimator enemyAnimator,
        IDamageTarget attacker,
        float attackDelay,
        int damagePerAttack
        ) : base(stateSwitcher, targetProvider, enemyAnimator)
    {
        _attacker = attacker;
        _attackDelay = attackDelay;
        _damagePerAttack = damagePerAttack;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        Observable.Interval(TimeSpan.FromSeconds(_attackDelay))
            .Subscribe(_ => PerformAttack())
            .AddTo(_disposables);
    }

    public override void OnTriggerExit(Collider other) 
    {
        var target = GetTarget();
        if (target == null)
            return;

        if (other.gameObject.name == target.gameObject.name)
            SwitchState<ApproachToTarget>();
    }

    private void PerformAttack()
    {
        var damageData = new DamageData()
        {
            DamageType = DamageType.Physical,
            BaseAmount = _damagePerAttack
        };

        Animator.PlayAttack();
        _attacker.DealDamage(GetTarget(), damageData);
    }
}
