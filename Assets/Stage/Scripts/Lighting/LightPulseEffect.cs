using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 光を一瞬輝かせて元に戻すパルス演出(0 敷物1の「光を輝く1秒」など)
// 設置のたびに繰り返し呼べる
[RequireComponent(typeof(Light2D))]
public class LightPulseEffect : MonoBehaviour
{
    [Header("パルス設定")]
    [Tooltip("最大の明るさ")]
    [Range(0f, 3f)]
    [SerializeField] private float peakIntensity = 1.5f;

    [Tooltip("明るくなるまでの時間(秒)")]
    [Range(0.05f, 2f)]
    [SerializeField] private float riseDuration = 0.25f;

    [Tooltip("最大の明るさを保つ時間(秒)")]
    [Range(0f, 2f)]
    [SerializeField] private float holdDuration = 0.5f;

    [Tooltip("元に戻るまでの時間(秒)")]
    [Range(0.05f, 2f)]
    [SerializeField] private float fallDuration = 0.25f;

    private Light2D light2D;
    private Coroutine pulseCoroutine;
    private float baseIntensity;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        baseIntensity = light2D.intensity;
    }

    // パルスを再生する(PlaceTargetのonCorrectItemPlacedなどに接続)
    public void Play()
    {
        if (!isActiveAndEnabled) return;
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        pulseCoroutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        yield return LerpIntensity(light2D.intensity, peakIntensity, riseDuration);
        yield return new WaitForSeconds(holdDuration);
        yield return LerpIntensity(light2D.intensity, baseIntensity, fallDuration);
        pulseCoroutine = null;
    }

    private IEnumerator LerpIntensity(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light2D.intensity = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        light2D.intensity = to;
    }
}