using UnityEngine;

public class FloatingDamageItemViewFactory
{
    private readonly Color CriticalColor = Color.red;

    private FloatingDamageItemView _itemPrefab;

    public FloatingDamageItemViewFactory(FloatingDamageItemView itemPrefab)
    {
        _itemPrefab = itemPrefab;
    }

    public FloatingDamageItemView Create(DamageData damage, Vector3 position)
    {
        FloatingDamageItemView floatingText = Object.Instantiate(_itemPrefab, position, Quaternion.identity);
        floatingText.SetText(damage.GetTotalDamageAmount().ToString());

        if (damage.IsCritical)
        {
            floatingText.SetTextColor(CriticalColor);
        }

        return floatingText;
    }
}
