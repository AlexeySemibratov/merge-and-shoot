using TMPro;
using UniRx;
using UnityEngine;

public class PlayerPanelPresenter : MonoBehaviour
{
    private const string KillCountText = "Enemies killed: {0}";
    private const string PlayerHpCountText = "HP: {0}";

    [SerializeField]
    private DamageTarget player;

    [SerializeField]
    private TextMeshProUGUI _enemyKilledText;

    [SerializeField]
    private TextMeshProUGUI _playerHpText;

    private int _killCount = 0;

    private void Start()
    {
        player.TargetKilled
            .ObserveOnMainThread()
            .Subscribe(_ => UpdateKillCount())
            .AddTo(this);

        player.CurrentHP
            .ObserveOnMainThread()
            .Subscribe(hp => UpdateHpText(hp))
            .AddTo(this);
    }

    private void UpdateKillCount()
    {
        _killCount++;
        _enemyKilledText.text = string.Format(KillCountText, _killCount);
    }

    private void UpdateHpText(int hp)
    {
        _playerHpText.text = string.Format(PlayerHpCountText, hp);
    }
}
