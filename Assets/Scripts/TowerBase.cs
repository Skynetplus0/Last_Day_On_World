using UnityEngine;

public abstract class TowerBase : MonoBehaviour
{
    [Header("Base Tower")]
    public float tickRate = 1f; // generic interval (attack, income, buff, etc.)
    public int cost = 100;

    float tickTimer;

    protected virtual void Update()
    {
        tickTimer -= Time.deltaTime;

        if (tickTimer <= 0f)
        {
            OnTick();
            tickTimer = tickRate;
        }
    }

    // 🔹 Child decides what a "tick" means
    protected abstract void OnTick();
}