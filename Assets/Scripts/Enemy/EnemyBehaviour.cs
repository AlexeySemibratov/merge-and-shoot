using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(DamageTarget), typeof(Rigidbody))]
public class EnemyBehaviour : MonoBehaviour
{
    public enum State
    {
        Approaching,
        Attacking,
        None
    }

    public IReadOnlyReactiveProperty<State> CurrentState => _currentState;

    public IObservable<Unit> Attacked => _attackSubject;

    [SerializeField]
    private float _movementSpeed = 1.0f;

    [SerializeField]
    private int _damagePerAttack = 25;

    [SerializeField]
    private float _attackDelay = 1.5f;

    private DamageTarget _owner;
    private Rigidbody _rigidBody;

    private ReactiveProperty<State> _currentState = new ReactiveProperty<State>(State.None);

    private Subject<Unit> _attackSubject = new();

    private DamageTarget _target;

    private void Awake()
    {
        InitializeComponents();
        ObserveCurrentState();
    }

    private void ObserveCurrentState()
    {
        _currentState
            .ObserveOnMainThread()
            .Subscribe(state => HandleState(state))
            .AddTo(this);
    }

    private void InitializeComponents()
    {
        _owner = GetComponent<DamageTarget>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void HandleState(State state)
    {
        switch (state)
        {
            case State.Approaching:
                ApproachToTarget();
                break;
            case State.Attacking:
                AttackTarget();
                break;
            case State.None:
                FindTarget();
                break;
            default:
                break;
        }
    }

    private void FindTarget()
    {
        // TODO Temporal solution for testing
        _target = Player.Instance.GetComponent<DamageTarget>();

        _currentState.Value = State.Approaching;
    }

    private void ApproachToTarget()
    {
        Observable.EveryFixedUpdate()
            .TakeWhile(_ => enabled && _currentState.Value == State.Approaching)
            .Subscribe(
                onNext: delta => MoveToTarget(delta),
                onCompleted: () => StopMovement())
            .AddTo(this);
    }

    private void MoveToTarget(float delta)
    {
        Vector3 direction = _target.transform.position - transform.position;
        _rigidBody.velocity = direction.normalized * _movementSpeed;

        Quaternion rotateTo = Quaternion.LookRotation(direction);
        _rigidBody.rotation = rotateTo;
    }

    private void StopMovement()
    {
        _rigidBody.velocity = Vector3.zero;
    }

    private void AttackTarget()
    {
        Observable.Interval(TimeSpan.FromSeconds(_attackDelay))
            .TakeWhile(_ => _currentState.Value == State.Attacking)
            .ObserveOnMainThread()
            .Subscribe(_ => PerformAttack())
            .AddTo(this);
    }

    private void PerformAttack()
    {
        _attackSubject.OnNext(Unit.Default);

        var damageData = new DamageData()
        {
            DamageType = DamageType.Physical,
            BaseAmount = _damagePerAttack
        };

        _owner.DealDamage(_target, damageData);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target == null)
            return;

        if (other.gameObject.name == _target.gameObject.name)
            _currentState.Value = State.Attacking;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target == null)
            return;

        if (other.gameObject.name == _target.gameObject.name)
        {
            _currentState.Value = State.Approaching;
        }
    }
}
