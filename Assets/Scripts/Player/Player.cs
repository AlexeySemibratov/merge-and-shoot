using UniRx;
using UnityEngine;

[RequireComponent(typeof(RifleWeapon))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(TargetFinder))]
public class Player : MonoBehaviour, IWorldMovementHandler, ITargetKeeper
{
    public static Player Instance { get; private set; }

    public IUpgradeable UpgradableWeapon => _rifleWeapon;

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

    public DamageTarget GetTarget()
    {
        return _currentTarget;
    }

    public void SetTarget(DamageTarget target)
    {
        _currentTarget = target;
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
        if (_damageTarget.IsDead)
            return;

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
        }
        else
        {
            _currentTarget = null;
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

    private void ObserveComponentEvents()
    {
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
        _rifleWeapon.Disable();
    }
}
