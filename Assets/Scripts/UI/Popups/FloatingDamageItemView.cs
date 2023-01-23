using TMPro;
using UnityEngine;

public class FloatingDamageItemView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetTextColor(Color color)
    {
        _text.color = color;
    }
}
