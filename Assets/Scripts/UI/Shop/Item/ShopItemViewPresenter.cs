using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItemViewPresenter : MonoBehaviour
{
    private const float EnabledAlpha = 1.0f;
    private const float DisabledAlpha = 0.3f;

    [SerializeField]
    private TextMeshProUGUI _costText;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private Button _itemButton;

    [SerializeField]
    private CanvasGroup _rootCanvas;

    public void UpdateData(ShopItemModel model)
    {
        _costText.text = model.Cost.ToString();
        _levelText.text = model.Level.ToString();
        _itemButton.interactable = model.Available;
        _rootCanvas.alpha = model.Available ? EnabledAlpha : DisabledAlpha;
    }

    public void AddItemOnClickAction(UnityAction action)
    {
        _itemButton.onClick.AddListener(action);
    }
}
