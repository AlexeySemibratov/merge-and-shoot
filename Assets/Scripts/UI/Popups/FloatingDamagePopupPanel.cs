using DG.Tweening;
using UnityEngine;

public class FloatingDamagePopupPanel : MonoBehaviour
{
    private const float Duration = 1.5f;

    [SerializeField]
    private FloatingDamageItemView _floatingDamageItemPrefab;

    private FloatingDamageItemViewFactory _itemFactory;

    private void Awake()
    {
        _itemFactory = new FloatingDamageItemViewFactory(_floatingDamageItemPrefab);
    }

    public void ShowFloatingDamage(Vector3 position, RecievedDamageData damageData)
    {
        FloatingDamageItemView floatingText = CreateFloatingText(position, damageData);
        AnimatePopup(floatingText);
        Destroy(floatingText.gameObject, Duration);
    }

    private FloatingDamageItemView CreateFloatingText(Vector3 posistion, RecievedDamageData damageData)
    {
        FloatingDamageItemView floatingText = _itemFactory.Create(damageData, posistion);
        floatingText.transform.SetParent(transform, false);
        return floatingText;
    }

    private void AnimatePopup(FloatingDamageItemView floatingText)
    {
        RectTransform rectTransform = floatingText.GetComponent<RectTransform>();

        rectTransform.localScale = Vector3.zero;

        rectTransform
            .DOScale(Vector3.one, 0.25f)
            .SetLink(floatingText.gameObject);

        rectTransform
            .DOAnchorPosY(40, Duration)
            .SetEase(Ease.InOutCubic)
            .SetLink(floatingText.gameObject);
    }
}
