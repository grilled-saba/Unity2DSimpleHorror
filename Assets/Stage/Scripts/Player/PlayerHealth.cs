using UnityEngine;
using System;

// プレイヤーのHP管理とダメージ処理
public class PlayerHealth : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private PlayerHealthData healthData;

    [Header("テスト用：危険度の直接指定")]
    [Tooltip("プレイ開始時に適用する危険度（テスト用）")]
    [SerializeField] private DangerLevel testDangerLevel = DangerLevel.None;

    private int currentHp;

    public event Action<int, int> OnHpChanged;
    public event Action OnDamaged;
    public event Action OnDead;

    public int CurrentHp => currentHp;
    public int MaxHp => healthData.MaxHp;
    public PlayerHealthData HealthData => healthData;

    private enum DangerLevel
    {
        None,
        Low,
        Mid,
        High
    }

    private void Awake()
    {
        currentHp = healthData.MaxHp;
        ApplyTestDangerLevel();
    }

    // テスト用危険度をHPに反映する
    private void ApplyTestDangerLevel()
    {
        switch (testDangerLevel)
        {
            case DangerLevel.Low:
                currentHp = Mathf.RoundToInt(healthData.MaxHp * 0.5f);
                break;
            case DangerLevel.Mid:
                currentHp = Mathf.RoundToInt(healthData.MaxHp * 0.2f);
                break;
            case DangerLevel.High:
                currentHp = Mathf.RoundToInt(healthData.MaxHp * 0.05f);
                break;
            default:
                return;
        }

        // テスト用危険度をイベントで通知する
        OnHpChanged?.Invoke(currentHp, healthData.MaxHp);
    }

    private void Start()
    {
        // 購読完了後に現在のHPを通知する
        OnHpChanged?.Invoke(currentHp, healthData.MaxHp);
    }
    public void TakeDamage(int amount)
    {
        if (currentHp <= 0) return;
        currentHp = Mathf.Max(0, currentHp - amount);
        Debug.Log($"ダメージを受けた: {amount} / 残りHP: {currentHp}");
        OnHpChanged?.Invoke(currentHp, healthData.MaxHp);
        OnDamaged?.Invoke();
        if (currentHp <= 0)
        {
            OnDead?.Invoke();
        }
    }
}