using UnityEngine;

public class HurtBoxWithFloatingDamage : Hurtbox
{
    [SerializeField]
    private FloatingDamagePopupPanel _floatingDamagePopup;

    [SerializeField]
    private Transform _floatingDamageShowPosition;

    public override void OnReceiveDamage(DamageData damageData)
    {
        base.OnReceiveDamage(damageData);

        _floatingDamagePopup.ShowFloatingDamage(_floatingDamageShowPosition.position, damageData);
    }
}
