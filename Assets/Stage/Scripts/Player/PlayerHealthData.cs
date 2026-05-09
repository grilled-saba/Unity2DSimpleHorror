using UnityEngine;

// プレイヤーのHP設定と危険度ごとの色データ
[CreateAssetMenu(fileName = "PlayerHealthData", menuName = "9T/Player/PlayerHealthData")]
public class PlayerHealthData : ScriptableObject
{
    [Header("HP設定")]
    [Tooltip("最大HP")]
    [Range(1, 500)]
    [SerializeField] private int maxHp = 100;

    [Header("危険度の色")]
    [Tooltip("通常状態の色")]
    [SerializeField] private Color normalColor = Color.white;

    [Tooltip("危険度：低（HP66%以下）")]
    [SerializeField] private Color lowDangerColor = Color.yellow;

    [Tooltip("危険度：中（HP33%以下）")]
    [SerializeField] private Color midDangerColor = new Color(1f, 0.5f, 0f);

    [Tooltip("危険度：高（HP10%以下）")]
    [SerializeField] private Color highDangerColor = Color.red;

    [Header("ダメージ演出設定")]
    [Tooltip("ダメージ時の点滅色")]
    [SerializeField] private Color damageFlashColor = Color.red;

    [Tooltip("点滅の持続時間（秒）")]
    [Range(0.1f, 5f)]
    [SerializeField] private float flashDuration = 1.5f;

    [Tooltip("点滅の間隔（秒）")]
    [Range(0.05f, 0.5f)]
    [SerializeField] private float flashInterval = 0.1f;

    [Tooltip("危険色へのブレンド時間（秒）")]
    [Range(0.1f, 3f)]
    [SerializeField] private float blendDuration = 0.5f;

    public int MaxHp => maxHp;
    public Color NormalColor => normalColor;
    public Color LowDangerColor => lowDangerColor;
    public Color MidDangerColor => midDangerColor;
    public Color HighDangerColor => highDangerColor;
    public Color DamageFlashColor => damageFlashColor;
    public float FlashDuration => flashDuration;
    public float FlashInterval => flashInterval;
    public float BlendDuration => blendDuration;

    // 現在HPに応じた危険度の色を返す
    public Color GetDangerColor(int currentHp)
    {
        float ratio = (float)currentHp / maxHp;

        if (ratio <= 0.1f) return highDangerColor;
        if (ratio <= 0.33f) return midDangerColor;
        if (ratio <= 0.66f) return lowDangerColor;
        return normalColor;
    }
}