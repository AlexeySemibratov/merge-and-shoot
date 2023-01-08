using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class Bullet : MonoBehaviour
{
    private const float LifeTimeSeconds = 2f;

    [SerializeField]
    private float _speed = 50f;

    private Hitbox _hitbox;
    private HitboxData _hitboxData;

    private Vector3 _direction => transform.forward.normalized;

    public void SetHitboxData(HitboxData data)
    {
        _hitbox.SetHitboxData(data);
    }

    private void Awake()
    {
        _hitbox = GetComponent<Hitbox>();
        _hitbox.SetHitboxData(_hitboxData);

        Destroy(gameObject, LifeTimeSeconds);
    }

    private void FixedUpdate()
    {
        transform.position += _direction * _speed * Time.fixedDeltaTime;
    }
}
