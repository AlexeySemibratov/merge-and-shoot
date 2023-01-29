using UniRx;
using UnityEngine;

[RequireComponent(typeof(DamageTarget), typeof(Rigidbody), typeof(EnemyAnimator))]
public class Enemy : MonoBehaviour
{
    public DamageTarget DamageTarget => _damageTarget;

    public int MoneyDropAmount => _moneyDropAmount;

    private const float DespawnTime = 2.0f;

    [SerializeField]
    private int _moneyDropAmount = 0;

    private DamageTarget _damageTarget;
    private Rigidbody _rigidBody;
    private EnemyAnimator _enemyAnimator;
    private EnemyBehaviour _enemyBehaviour;

    private void Awake()
    {
        InitializeComponents();
        ObserveDamageTargetEvents();
        ObserveEnemyBehaviorEvents();
    }

    private void InitializeComponents()
    {
        _enemyAnimator = GetComponent<EnemyAnimator>();
        _damageTarget = GetComponent<DamageTarget>();
        _rigidBody = GetComponent<Rigidbody>();
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

    private void ObserveEnemyBehaviorEvents()
    {
        _enemyBehaviour.CurrentState
            .Subscribe(state => AnimateBehaviour(state))
            .AddTo(this);

        _enemyBehaviour.Attacked
            .Subscribe(_ => _enemyAnimator.PlayAttack())
            .AddTo(this);
    }

    private void AnimateBehaviour(EnemyBehaviour.State state)
    {
        switch (state)
        {
            case EnemyBehaviour.State.Approaching:
                _enemyAnimator.ChangeStateTo(EnemyAnimationState.Walking);
                break;
            default:
                _enemyAnimator.ChangeStateTo(EnemyAnimationState.Idle);
                break;
        }
    }

    private void OnTakeDamage(DamageTakenEvent damageEvent)
    {
        if (damageEvent.RecievedDamage.IsDeadlyDamage == false)
            _enemyAnimator.PlayTakeDamage();
    }

    private void OnDead(IDamageTarget target)
    {
        _enemyAnimator.ChangeStateTo(EnemyAnimationState.Dead);
        _enemyBehaviour.enabled = false;
        DisableBodyPhysics();
        DespawnEnemy();
    }

    private void DisableBodyPhysics()
    {
        _rigidBody.detectCollisions = false;
        _rigidBody.velocity = Vector3.zero;

        Destroy(_enemyBehaviour);
    }

    private void DespawnEnemy()
    {
        Destroy(gameObject, DespawnTime);
    }
}
