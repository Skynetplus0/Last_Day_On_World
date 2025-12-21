using UnityEngine;

public class MoneyTower : TowerBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Economy")]
    public int moneyPerTick = 10;

    protected override void OnTick()
    {
        CoinManager.Instance.AddCoins(moneyPerTick);
    }
}
