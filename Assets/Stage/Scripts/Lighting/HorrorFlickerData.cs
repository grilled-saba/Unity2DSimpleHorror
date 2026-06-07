using UnityEngine;

// ブラックアウト(完全消灯)を伴うホラー風明滅の数値を管理するScriptableObject
// 通常のFlickerLight/LightingDataとは別物。トリガーで点灯する光源向け
[CreateAssetMenu(fileName = "HorrorFlickerData", menuName = "9T/Lighting/HorrorFlickerData")]
public class HorrorFlickerData : ScriptableObject
{
    [Header("点灯時の明るさ")]
    [Tooltip("通常点灯時の強度。初期intensityに依存せず常にこの値を基準にする")]
    [Range(0f, 3f)]
    [SerializeField] private float onIntensity = 1.5f;

    [Header("安定フェーズ")]
    [Tooltip("点灯中の微弱なゆらぎ量(0で完全に安定)")]
    [Range(0f, 1f)]
    [SerializeField] private float subtleFlickerAmount = 0.1f;

    [Tooltip("ブラックアウトまでの安定時間の最小(秒)")]
    [Range(0.1f, 20f)]
    [SerializeField] private float stableDurationMin = 2f;

    [Tooltip("ブラックアウトまでの安定時間の最大(秒)")]
    [Range(0.1f, 20f)]
    [SerializeField] private float stableDurationMax = 6f;

    [Header("ブラックアウト(完全消灯)")]
    [Tooltip("消灯時間の最小(秒)")]
    [Range(0.02f, 3f)]
    [SerializeField] private float blackoutDurationMin = 0.05f;

    [Tooltip("消灯時間の最大(秒)")]
    [Range(0.02f, 3f)]
    [SerializeField] private float blackoutDurationMax = 0.4f;

    [Header("スタッター(消える前の高速明滅)")]
    [Tooltip("ブラックアウト前の高速明滅回数(0でなし)")]
    [Range(0, 8)]
    [SerializeField] private int stutterCount = 2;

    [Tooltip("高速明滅の1回あたりの間隔(秒)")]
    [Range(0.01f, 0.3f)]
    [SerializeField] private float stutterInterval = 0.05f;

    [Header("点灯・消灯の遷移")]
    [Tooltip("点灯時のフェードイン時間(秒)")]
    [Range(0f, 3f)]
    [SerializeField] private float fadeInDuration = 0.3f;

    [Tooltip("消灯時のフェードアウト時間(秒)")]
    [Range(0f, 3f)]
    [SerializeField] private float fadeOutDuration = 0.5f;

    public float OnIntensity => onIntensity;
    public float SubtleFlickerAmount => subtleFlickerAmount;
    public float StableDurationMin => stableDurationMin;
    public float StableDurationMax => stableDurationMax;
    public float BlackoutDurationMin => blackoutDurationMin;
    public float BlackoutDurationMax => blackoutDurationMax;
    public int StutterCount => stutterCount;
    public float StutterInterval => stutterInterval;
    public float FadeInDuration => fadeInDuration;
    public float FadeOutDuration => fadeOutDuration;
}