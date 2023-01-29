using System;
using UniRx;

public interface IDamageTarget
{
    IObservable<DamageDealedEvent> DamageDealed { get; }

    IObservable<DamageTakenEvent> DamageTaken { get; }

    IObservable<IDamageTarget> TargetDead { get; }

    IObservable<Unit> TargetKilled { get; }

    int MaxHP { get; }

    IntReactiveProperty CurrentHP { get; }

    bool IsDead { get; }

    void DealDamage(IDamageTarget target, DamageData damage);

    void TakeDamage(IDamageTarget from, DamageData damage);
}
