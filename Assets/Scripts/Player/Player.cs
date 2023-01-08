using UniRx;
using UnityEngine;

[RequireComponent(typeof(RifleWeapon))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(TargetFinder))]
public class Player : MonoBehaviour, IWorldMovementHandler
{
    public static Player Instance { get; private set; }

    [SerializeField]
    private float _rotationSpeed = 300f;

    [SerializeField]
    private Transform _body;

    private RifleWeapon _rifleWeapon;
    private PlayerAnimator _animator;
    private DamageTarget _damageTarget;
    private TargetFinder _targetFinder;

    private DamageTarget _currentTarget;

    private void Awake()
    {
        Instance = this;

        InitializeComponents();
        ObserveComponentEvents();
    }

    public void OnWorldStartMovement()
    {
        _animator.ChangeAnimationStateTo(PlayerAnimationState.Walk);
    }

    public void OnWorldEndMovement()
    {
        _animator.ChangeAnimationStateTo(PlayerAnimationState.Idle);
    }

    private void InitializeComponents()
    {
        _rifleWeapon = GetComponent<RifleWeapon>();
        _animator = GetComponent<PlayerAnimator>();
        _damageTarget = GetComponent<DamageTarget>();
        _targetFinder = GetComponent<TargetFinder>();
    }

    private void Update()
    {
        UpdateBehaviour();
    }

    private void UpdateBehaviour()
    {
        if (_currentTarget == null)
        {
            FindTarget();
        } 
        else if (_currentTarget.IsDead == false)
        {
            RotateToTarget(_currentTarget);
            ShootToTarget(_currentTarget);
        }
    }

    private void FindTarget()
    {
        _currentTarget = _targetFinder.FindTarget();
    }

    private void RotateToTarget(DamageTarget target)
    {
        Vector3 direction = _body.position.DirectedTo(target.transform.position);

        Quaternion rotateTo = Quaternion.LookRotation(direction);
        _body.rotation = Quaternion.RotateTowards(_body.rotation, rotateTo, _rotationSpeed * Time.fixedDeltaTime);
    }

    private void ShootToTarget(DamageTarget target)
    {
        _rifleWeapon.ShootAt(target);
    }

    private void ObserveComponentEvents()
    {
        _rifleWeapon.SingleShootPerformed
            .Subscribe(_ => PlayShootAnimation())
            .AddTo(this);

        _damageTarget.TargetDead
            .Subscribe(_ => OnDead())
            .AddTo(this);
    }

    private void OnDead()
    {
        _animator.ChangeAnimationStateTo(PlayerAnimationState.Dead);

        DisableComponents();
    }

    private void DisableComponents()
    {
        _rifleWeapon.enabled = false;
    }

    private void PlayShootAnimation()
    {
        _animator.PlayShoot();
    }
}
