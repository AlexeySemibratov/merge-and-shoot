using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingDamagePopup : MonoBehaviour
{
    private const float Duration = 1.5f;

    [SerializeField]
    private TextMeshProUGUI _floatingTextPrefab;

    public void ShowFloatingDamage(Vector3 position, int amount)
    {
        TextMeshProUGUI floatingText = CreateFloatingText(position, amount);
        AnimatePopup(floatingText);
        Destroy(floatingText.gameObject, Duration);
    }

    private TextMeshProUGUI CreateFloatingText(Vector3 posistion, int amount)
    {
        TextMeshProUGUI floatingText = Instantiate(_floatingTextPrefab, posistion, Quaternion.identity);
        floatingText.transform.SetParent(transform, false);
        floatingText.text = amount.ToString();
        return floatingText;
    }

    private void AnimatePopup(TextMeshProUGUI floatingText)
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
