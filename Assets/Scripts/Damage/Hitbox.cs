using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public HitboxData HitboxData { get; private set; }

    [SerializeField]
    private bool _destroyGameObjectWhenHit = false;

    private bool _hitTarget = false;

    public void SetHitboxData(HitboxData data)
    {
        HitboxData = data;  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hitTarget == false && other.gameObject.TryGetComponent(out Hurtbox hurtbox))
        {
            _hitTarget = true;
            OnHitTarget(hurtbox.Owner);
            hurtbox.OnReceiveDamage(HitboxData.DamageAmount);
        }
    }

    private void OnHitTarget(IDamageTarget target)
    {
        HitboxData.Owner.DealDamage(target, HitboxData.DamageAmount);

        if (_destroyGameObjectWhenHit)
        {
            Destroy(gameObject);
        }
    }
}
