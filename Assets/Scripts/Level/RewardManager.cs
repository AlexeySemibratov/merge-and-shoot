using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private Shop _shop => GameContext.Instance.Shop;

    public void AddRewardForEnemy(int moneyAmount)
    {
        _shop.AddMoney(moneyAmount);
    }
}
