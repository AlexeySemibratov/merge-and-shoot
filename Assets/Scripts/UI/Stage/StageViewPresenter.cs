using TMPro;
using UniRx;
using UnityEngine;

public class StageViewPresenter : MonoBehaviour
{
    private const string LevelText = "Level {0}";

    [SerializeField]
    private TextMeshProUGUI _stageText;

    [SerializeField]
    private LevelManager _levelManager;

    private void Awake()
    {
        _levelManager.CurrentLevel
            .Subscribe(level => UpdateLevelText(level))
            .AddTo(this);
    }

    private void UpdateLevelText(int level)
    {
        _stageText.text = string.Format(LevelText, level);
    }
}
