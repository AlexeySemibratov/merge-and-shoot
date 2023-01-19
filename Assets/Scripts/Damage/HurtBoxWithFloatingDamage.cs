using UnityEngine;

public class HurtBoxWithFloatingDamage : Hurtbox
{
    [SerializeField]
    private FloatingDamagePopup _floatingDamagePopup;

    [SerializeField]
    private Transform _floatingDamageShowPosition;

    public override void OnReceiveDamage(int damageAmount)
    {
        base.OnReceiveDamage(damageAmount);

        _floatingDamagePopup.ShowFloatingDamage(_floatingDamageShowPosition.position, damageAmount);
    }
}
