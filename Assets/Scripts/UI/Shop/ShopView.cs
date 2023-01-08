using System;
using System.Data.SqlTypes;
using UniRx;
using UnityEngine;

public class ShopView : MonoBehaviour
{
    [SerializeField]
    private ShopItemViewPresenter _weaponUpgradeItem;

    [SerializeField]
    private Shop _shop;

    private void Awake()
    {
        SetupItems();
        ObserveShopEvents();
    }

    private void SetupItems()
    {
        _weaponUpgradeItem.AddItemOnClickAction(BuyWeaponUpgrade);
    }

    private void BuyWeaponUpgrade()
    {
        _shop.BuyWeaponUpgrade();
    }

    private void ObserveShopEvents()
    {
        IObservable<Tuple<int, UpgradeItem>> source = Observable.CombineLatest(
            _shop.CurrentMoney,
            _shop.AvailableWeaponUpgrade,
            (money, upgrade) => Tuple.Create(money, upgrade));

            source.Subscribe(tuple => UpdateWeaponUpgradeItemData(tuple.Item1, tuple.Item2))
            .AddTo(this);

        _shop.CurrentMoney
            .Subscribe(amount => UpdateMoneyAmount(amount))
            .AddTo(this);
    }

    private void UpdateMoneyAmount(int amount)
    {
        Debug.Log($"Money amount {amount}!");
    }

    private void UpdateWeaponUpgradeItemData(int currentMoney, UpgradeItem upgrade)
    {
        Debug.Log("Update weapon view data!");
        var model = new ShopItemModel
        {
            Cost = upgrade.Cost,
            Level = upgrade.Level,
            Available = upgrade.Cost <= currentMoney
        };

        _weaponUpgradeItem.UpdateData(model);
    }
}
