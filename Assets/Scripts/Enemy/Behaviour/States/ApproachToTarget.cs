using UniRx;
using UnityEngine;

public class ApproachToTarget : EnemyStateBase
{
    private Rigidbody _rigidBody;
    private Transform _transform;
    private float _movementSpeed;

    public ApproachToTarget(
        IEnemyStateSwitcher stateSwitcher, 
        ITargetProvider targetProvider,
        EnemyAnimator enemyAnimator,
        Rigidbody rigidBody,
        Transform transform,
        float movementSpeed
        ) : base(stateSwitcher, targetProvider, enemyAnimator)
    {
        _rigidBody = rigidBody;
        _transform = transform;
        _movementSpeed = movementSpeed;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        Observable.EveryFixedUpdate()
            .Subscribe(delta => MoveToTarget())
            .AddTo(_disposables);

        Animator.ChangeStateTo(EnemyAnimationState.Walking);
    }

    public override void OnStateExit()
    {
        base.OnStateExit();

        _rigidBody.velocity = Vector3.zero;
        Animator.ChangeStateTo(EnemyAnimationState.Idle);
    }

    public override void OnStateDestroyed()
    {
        base.OnStateDestroyed();

        _rigidBody.velocity = Vector3.zero;
        _rigidBody.detectCollisions = false;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        var target = GetTarget();

        if (target == null)
            return;

        if (other.gameObject.name == target.gameObject.name)
            StateSwitcher.SwitchStateTo<AttackTarget>();
    }

    private void MoveToTarget()
    {
        var target = GetTarget();

        if (target == null)
            return;

        Vector3 direction = target.transform.position - _transform.position;
        _rigidBody.velocity = direction.normalized * _movementSpeed;

        Quaternion rotateTo = Quaternion.LookRotation(direction);
        _rigidBody.rotation = rotateTo;
    }
}
