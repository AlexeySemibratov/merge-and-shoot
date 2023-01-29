public interface ITargetProvider
{
    void SetTarget(DamageTarget target);

    DamageTarget GetTarget();
}
