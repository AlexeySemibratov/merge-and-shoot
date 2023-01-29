using UniRx;
using UnityEngine;

[RequireComponent(typeof(DamageTarget), typeof(EnemyAnimator))]
public class Enemy : MonoBehaviour
{
    public DamageTarget DamageTarget => _damageTarget;

    public int MoneyDropAmount => _moneyDropAmount;

    private const float DespawnTime = 2.0f;

    [SerializeField]
    private int _moneyDropAmount = 0;

    private DamageTarget _damageTarget;
    private EnemyAnimator _enemyAnimator;
    private EnemyBehaviour _enemyBehaviour;

    private void Awake()
    {
        InitializeComponents();
        ObserveDamageTargetEvents();
    }

    private void InitializeComponents()
    {
        _enemyAnimator = GetComponent<EnemyAnimator>();
        _damageTarget = GetComponent<DamageTarget>();
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

    private void ObserveDamageTargetEvents()
    {
        _damageTarget.TargetDead
            .Subscribe(target => OnDead(target))
            .AddTo(this);

        _damageTarget.DamageTaken
            .Subscribe(damageEvent => OnTakeDamage(damageEvent))
            .AddTo(this);   
    }

    private void OnTakeDamage(DamageTakenEvent damageEvent)
    {
        if (damageEvent.RecievedDamage.IsDeadlyDamage == false)
            _enemyAnimator.PlayTakeDamage();
    }

    private void OnDead(IDamageTarget target)
    {
        _enemyAnimator.ChangeStateTo(EnemyAnimationState.Dead);
        DespawnEnemy();
    }

    private void DespawnEnemy()
    {
        Destroy(gameObject, DespawnTime);
    }
}
