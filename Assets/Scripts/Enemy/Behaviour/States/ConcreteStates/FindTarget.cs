public class FindTarget : EnemyStateBase
{
    public FindTarget(
        IEnemyStateSwitcher stateSwitcher,
        ITargetProvider targetProvider,
        EnemyAnimator enemyAnimator
        ) : base(stateSwitcher, targetProvider, enemyAnimator)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        if (GetTarget() == null)
            SetTarget(Player.Instance.GetComponent<DamageTarget>());

        StateSwitcher.SwitchStateTo<ApproachToTarget>();
    }
}
