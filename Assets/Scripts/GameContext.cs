using UnityEngine;

public class GameContext : MonoBehaviour
{
    public static GameContext Instance { get; private set; }

    public LevelManager LevelManager;

    public RewardManager RewardManager;

    public Shop Shop;

    public Player Player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Trying to create another GameContext instance");
            Destroy(gameObject);
        }
    }
}
