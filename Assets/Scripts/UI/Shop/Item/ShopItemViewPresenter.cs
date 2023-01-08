using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItemViewPresenter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _costText;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private Button _itemButton;

    public void UpdateData(ShopItemModel model)
    {
        _costText.text = model.Cost.ToString();
        _levelText.text = model.Level.ToString();
        _itemButton.enabled = model.Available;
    }

    public void AddItemOnClickAction(UnityAction action)
    {
        _itemButton.onClick.AddListener(action);
    }
}
