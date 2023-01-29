using System.Collections;
using UniRx;
using UnityEngine;

public class FloatingDamageTarget : MonoBehaviour
{
    [SerializeField]
    private DamageTarget _target;

    [SerializeField]
    private FloatingDamagePopupPanel _floatingDamagePopup;

    [SerializeField]
    private Transform _floatingDamageShowPosition;

    private void Awake()
    {
        ObserveDamageEvents();
    }

    private void ObserveDamageEvents()
    {
        _target.DamageTaken
            .Subscribe(e => OnDamageTaken(e))
            .AddTo(this);
    }

    private void OnDamageTaken(DamageTakenEvent damageEvent)
    {
        _floatingDamagePopup.ShowFloatingDamage(_floatingDamageShowPosition.position, damageEvent.RecievedDamage);
    }
}
