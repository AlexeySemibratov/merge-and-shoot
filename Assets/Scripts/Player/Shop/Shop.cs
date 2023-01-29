using System;
using UniRx;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public IReadOnlyReactiveProperty<int> CurrentMoney => _currentMoney;

    public IObservable<UpgradeItem> AvailableWeaponUpgrade => _availableWeaponUpgradeSubject;
    private ISubject<UpgradeItem> _availableWeaponUpgradeSubject = new ReplaySubject<UpgradeItem>();

    private IntReactiveProperty _currentMoney = new IntReactiveProperty(1000);

    private UpgradeItem _availableWeaponUpgrade = new UpgradeItem(1, 50);

    private Player _player => Player.Instance;

    private void Start()
    {
        _availableWeaponUpgradeSubject.OnNext(_availableWeaponUpgrade);
    }

    public void BuyWeaponUpgrade()
    {
        if (_currentMoney.Value >= _availableWeaponUpgrade.Cost)
        {
            RemoveMoney(_availableWeaponUpgrade.Cost);
            _player.UpgradableWeapon.ApplyUpgrade(_availableWeaponUpgrade);
            UpdateWeaponUpgrade();
        }
    }

    public void AddMoney(int value)
    {
        _currentMoney.Value = _currentMoney.Value + value;
    }

    private void UpdateWeaponUpgrade()
    {
        int previousCost = _availableWeaponUpgrade.Cost;

        _availableWeaponUpgrade = new UpgradeItem(
            _availableWeaponUpgrade.Level + 1,
            (int)(previousCost * 1.5));

        _availableWeaponUpgradeSubject.OnNext(_availableWeaponUpgrade);
    }

    private void RemoveMoney(int value)
    {
        _currentMoney.Value = _currentMoney.Value - value;
    }
}
