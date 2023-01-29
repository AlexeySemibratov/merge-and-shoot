using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(DamageTarget), typeof(Rigidbody), typeof(EnemyAnimator))]
public class EnemyBehaviour : MonoBehaviour, IEnemyStateSwitcher, ITargetProvider
{
    [SerializeField]
    private float _movementSpeed = 1.0f;

    [SerializeField]
    private int _damagePerAttack = 25;

    [SerializeField]
    private float _attackDelay = 1.5f;

    private DamageTarget _owner;
    private Rigidbody _rigidBody;
    private EnemyAnimator _enemyAnimator;

    private DamageTarget _currentTarget;

    private List<IEnemyState> _states;

    private IEnemyState _currentState;

    private void Awake()
    {
        InitializeComponents();
        InitializeStates();
        ObserveDeadEvent();
    }

    private void InitializeStates()
    {
        _states = new List<IEnemyState>() {
            new FindTarget(this, this, _enemyAnimator),
            new ApproachToTarget(this, this, _enemyAnimator, _rigidBody, transform, _movementSpeed),
            new AttackTarget(this, this, _enemyAnimator, _owner, _attackDelay, _damagePerAttack)
        };

        SwitchStateTo<FindTarget>();
    }

    public void SwitchStateTo<T>() where T : IEnemyState
    {
        var type = typeof(T);
        var newState = _states.FirstOrDefault(e => e.GetType() == type);

        if (newState == null)
            throw new ArgumentException($"State {type} not found");

        if (_currentState != null)
            _currentState.OnStateExit();

        _currentState = newState;
        _currentState.OnStateEnter();
    }

    public void SetTarget(DamageTarget target)
    {
        _currentTarget = target;
    }

    public DamageTarget GetTarget()
    {
        return _currentTarget;
    }

    private void InitializeComponents()
    {
        _owner = GetComponent<DamageTarget>();
        _rigidBody = GetComponent<Rigidbody>();
        _enemyAnimator = GetComponent<EnemyAnimator>();
    }

    private void ObserveDeadEvent()
    {
        _owner.TargetDead
            .Subscribe(_ => DestroyStates())
            .AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_currentState != null)
            _currentState.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_currentState != null)
            _currentState.OnTriggerExit(other);
    }

    private void DestroyStates()
    {
        _states.DoForEach(state => state.OnStateDestroyed());
        _states.Clear();
        _currentState = null;
    }
}
