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

    public void DealDamage(IDamageTarget target, DamageData damage)
    {
        if (IsDead) return;

        target.TakeDamage(this, damage);

        var damageEvent = new DamageDealedEvent 
        {
            Target = target,
            Damage = damage
        };
        
        _damageDealedSubject.OnNext(damageEvent);

        CheckTargetWasKilled(target);
    }

    private void CheckTargetWasKilled(IDamageTarget target)
    {
        if (target.IsDead)
            _targetKilledSubject.OnNext(Unit.Default);
    }

    public void TakeDamage(IDamageTarget from, DamageData damage)
    {
        if (IsDead) 
            return;

        int hpBeforeDamage = CurrentHP.Value;
        CurrentHP.Value = Math.Clamp(CurrentHP.Value - damage.GetTotalDamageAmount(), MinHP, MaxHP);

        int recievedDamage = hpBeforeDamage - CurrentHP.Value;

        EmitDamageTakenEvent(from, damage, recievedDamage);

        if (IsDead)
            OnDead();
    }

    private void EmitDamageTakenEvent(IDamageTarget from, DamageData sourceDamage, int recievedDamage)
    {
        bool isDeadlyDamage = IsDead;

        var recievedDamageData = new RecievedDamageData()
        {
            DamageType = sourceDamage.DamageType,
            Amount = recievedDamage,
            IsCritical = sourceDamage.IsCritical,
            IsDeadlyDamage = isDeadlyDamage
        };

        var damageEvent = new DamageTakenEvent
        {
            From = from,
            SourceDamage = sourceDamage,
            RecievedDamage = recievedDamageData
        };

        _damageTakenSubject.OnNext(damageEvent);
    }

    private void OnDead()
    {
        _targetDeadSubject.OnNext(this);
    }
}
