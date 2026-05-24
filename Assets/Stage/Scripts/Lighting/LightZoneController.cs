using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 指定したLight2D群を指定強度・色へフェードさせる
public class LightZoneController : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private LightingData lightingData;

    [Header("制御対象")]
    [Tooltip("このゾーンで制御するLight2Dの配列")]
    [SerializeField] private Light2D[] targetLights;

    [Header("目標値")]
    [Range(0f, 2f)]
    [SerializeField] private float targetIntensity = 0.05f;

    [SerializeField] private Color targetColor = Color.white;

    private Coroutine fadeCoroutine;

    // 外部から目標値へフェード開始
    public void FadeToTarget()
    {
        if (!isActiveAndEnabled) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetIntensity, targetColor));
    }

    // 異常状態(赤)へ即フェード
    public void FadeToAlert()
    {
        if (!isActiveAndEnabled) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetIntensity, lightingData.AlertColor));
    }

    // 消灯状態(Intensity 0)へフェード。色は現在値を維持する
    public void FadeToOff()
    {
        if (!isActiveAndEnabled) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOffRoutine());
    }

    private IEnumerator FadeRoutine(float toIntensity, Color toColor)
    {
        // 各ライトの開始値を保存
        float[] startIntensities = new float[targetLights.Length];
        Color[] startColors = new Color[targetLights.Length];

        for (int i = 0; i < targetLights.Length; i++)
        {
            startIntensities[i] = targetLights[i].intensity;
            startColors[i] = targetLights[i].color;
        }

        float elapsed = 0f;
        float duration = lightingData.TransitionDuration;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // 緩急のあるイージング(SmoothStep)で自然な遷移
            float eased = t * t * (3f - 2f * t);

            for (int i = 0; i < targetLights.Length; i++)
            {
                targetLights[i].intensity = Mathf.Lerp(startIntensities[i], toIntensity, eased);
                targetLights[i].color = Color.Lerp(startColors[i], toColor, eased);
            }

            yield return null;
        }
    }

    // 色を維持したままIntensityのみ0にフェードするコルーチン
    private IEnumerator FadeOffRoutine()
    {
        float[] startIntensities = new float[targetLights.Length];
        for (int i = 0; i < targetLights.Length; i++)
        {
            startIntensities[i] = targetLights[i].intensity;
        }

        float elapsed = 0f;
        float duration = lightingData.TransitionDuration;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = t * t * (3f - 2f * t);

            for (int i = 0; i < targetLights.Length; i++)
            {
                targetLights[i].intensity = Mathf.Lerp(startIntensities[i], 0f, eased);
            }

            yield return null;
        }
    }
}