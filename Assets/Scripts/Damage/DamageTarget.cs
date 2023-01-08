using System;
using UniRx;
using UnityEngine;

public class DamageTarget : MonoBehaviour, IDamageTarget
{
    public IObservable<DamageDealedEvent> DamageDealed { get => _damageDealedSubject; }

    public IObservable<DamageTakenEvent> DamageTaken { get => _damageTakenSubject; }

    public IObservable<IDamageTarget> TargetDead { get => _targetDeadSubject; }

    public IObservable<Unit> TargetKilled { get => _targetKilledSubject; }

    private Subject<Unit> _targetKilledSubject = new();
    private Subject<IDamageTarget> _targetDeadSubject = new();
    private Subject<DamageDealedEvent> _damageDealedSubject = new();
    private Subject<DamageTakenEvent> _damageTakenSubject = new();

    private const int MinHP = 0;

    public int MaxHP { get => _maxHP; }

    public IntReactiveProperty CurrentHP { get; private set; }

    public bool IsDead { get => CurrentHP.Value <= MinHP; }

    [SerializeField]
    private int _maxHP;

    private void Awake()
    {
        CurrentHP = new IntReactiveProperty(MaxHP);
    }

    public void DealDamage(IDamageTarget target, int damageAmount)
    {
        if (IsDead) return;

        target.TakeDamage(this, damageAmount);

        var damageEvent = new DamageDealedEvent 
        {
            Target = target,
            Damage = damageAmount
        };
        
        _damageDealedSubject.OnNext(damageEvent);

        CheckTargetWasKilled(target);
    }

    private void CheckTargetWasKilled(IDamageTarget target)
    {
        if (target.IsDead)
        {
            _targetKilledSubject.OnNext(Unit.Default);
        }
    }

    public void TakeDamage(IDamageTarget from, int damageAmount)
    {
        if (IsDead) return;

        CurrentHP.Value = Math.Clamp(CurrentHP.Value - damageAmount, MinHP, MaxHP);
        bool isDeadlyDamage = IsDead;

        var damageEvent = new DamageTakenEvent
        {
            From = from,
            Damage = damageAmount,
            IsDeadlyDamage = isDeadlyDamage
        };

        _damageTakenSubject.OnNext(damageEvent);

        if (isDeadlyDamage)
        {
            OnDead();
        }
    }

    private void OnDead()
    {
        _targetDeadSubject.OnNext(this);
    }
}
