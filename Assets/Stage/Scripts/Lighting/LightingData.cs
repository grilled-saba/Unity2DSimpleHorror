using UnityEngine;

// 光源演出に関わる数値を一括管理するScriptableObject
[CreateAssetMenu(fileName = "LightingData", menuName = "9T/Lighting/LightingData")]
public class LightingData : ScriptableObject
{
    [Header("基準値")]
    [Tooltip("通常状態の周囲光の明るさ")]
    [Range(0f, 1f)]
    [SerializeField] private float baseIntensity = 0.1f;

    [Tooltip("通常状態の周囲光の色")]
    [SerializeField] private Color baseColor = new Color(0.04f, 0.07f, 0.13f);

    [Header("ちらつき設定")]
    [Tooltip("ちらつきの最小強度倍率")]
    [Range(0f, 1f)]
    [SerializeField] private float flickerMinRatio = 0.4f;

    [Tooltip("ちらつきの最大強度倍率")]
    [Range(0f, 2f)]
    [SerializeField] private float flickerMaxRatio = 1.1f;

    [Tooltip("ちらつき間隔の最小秒数")]
    [Range(0.01f, 1f)]
    [SerializeField] private float flickerIntervalMin = 0.05f;

    [Tooltip("ちらつき間隔の最大秒数")]
    [Range(0.01f, 2f)]
    [SerializeField] private float flickerIntervalMax = 0.25f;

    [Header("危険演出")]
    [Tooltip("異常事態に切り替わる赤色")]
    [SerializeField] private Color alertColor = new Color(0.9f, 0.1f, 0.15f);

    [Tooltip("通常から異常への遷移時間")]
    [Range(0.1f, 5f)]
    [SerializeField] private float transitionDuration = 0.8f;

    // 外部公開プロパティ
    public float BaseIntensity => baseIntensity;
    public Color BaseColor => baseColor;
    public float FlickerMinRatio => flickerMinRatio;
    public float FlickerMaxRatio => flickerMaxRatio;
    public float FlickerIntervalMin => flickerIntervalMin;
    public float FlickerIntervalMax => flickerIntervalMax;
    public Color AlertColor => alertColor;
    public float TransitionDuration => transitionDuration;
}