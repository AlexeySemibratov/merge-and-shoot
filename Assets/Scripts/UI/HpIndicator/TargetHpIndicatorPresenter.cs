using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TargetHpIndicatorPresenter : MonoBehaviour
{
    [SerializeField]
    private DamageTarget _target;

    [SerializeField]
    private Slider _hpSlider;

    private void Start()
    {
        SetupView();
        ObserveHpChanged();
    }

    private void SetupView()
    {
        _hpSlider.maxValue = _target.MaxHP;
        _hpSlider.value = _target.MaxHP;
    }

    private void ObserveHpChanged()
    {
        _target.CurrentHP
            .ObserveOnMainThread()
            .Subscribe(hp => UpdateHpSliderValue(hp));
    }

    private void UpdateHpSliderValue(int hp)
    {
        _hpSlider.value = hp;

        if (hp <= 0)
        {
            Destroy(gameObject);   
        }
    }
}
