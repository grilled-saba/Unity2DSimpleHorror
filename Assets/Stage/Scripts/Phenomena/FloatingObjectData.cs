using UnityEngine;

// 浮遊演出に関する数値データ
[CreateAssetMenu(fileName = "FloatingObjectData", menuName = "9T/Phenomena/FloatingObjectData")]
public class FloatingObjectData : ScriptableObject
{
    [Header("浮遊設定")]
    [Tooltip("浮遊までにかかる時間（秒）")]
    [Range(0.1f, 5f)]
    [SerializeField] private float floatDuration = 2f;

    [Tooltip("浮遊する高さ")]
    [Range(0.5f, 10f)]
    [SerializeField] private float floatHeight = 3f;

    [Tooltip("浮遊状態の維持時間（秒）")]
    [Range(0.1f, 5f)]
    [SerializeField] private float floatHoldDuration = 1f;

    [Header("AOE設定")]
    [Tooltip("オブジェクトサイズに対するAOE範囲の倍率")]
    [Range(0.5f, 5f)]
    [SerializeField] private float aoeRadiusMultiplier = 2f;

    [Tooltip("AOEのダメージ量")]
    [Range(1, 100)]
    [SerializeField] private int aoeDamage = 20;

    public float FloatDuration => floatDuration;
    public float FloatHeight => floatHeight;
    public float FloatHoldDuration => floatHoldDuration;
    public float AoeRadiusMultiplier => aoeRadiusMultiplier;
    public int AoeDamage => aoeDamage;
}