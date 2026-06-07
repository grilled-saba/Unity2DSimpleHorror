using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

// ブラックアウトを伴うホラー風明滅光源
// 光源のintensityを単独で所有し、点灯・明滅・消灯まで自前で行う
// LightZoneControllerとは併用しない
[RequireComponent(typeof(Light2D))]
public class HorrorFlickerLight : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private HorrorFlickerData data;

    [Header("動作設定")]
    [Tooltip("起動時から点灯・明滅を始めるか")]
    [SerializeField] private bool flickerOnStart = false;

    [Header("イベント")]
    [Tooltip("DieOut完了後に発火")]
    [SerializeField] private UnityEvent onDied;

    private Light2D light2D;
    private Coroutine mainCoroutine;
    private float seed;
    private bool isDead;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        light2D.intensity = 0f;
        seed = Random.value * 100f;
    }

    private void Start()
    {
        if (flickerOnStart) TurnOn();
    }

    // 点灯して明滅を開始する。繰り返し点滅させたい光源に使う
    public void TurnOn()
    {
        if (!isActiveAndEnabled) return;
        if (isDead) return;
        if (mainCoroutine != null) StopCoroutine(mainCoroutine);
        mainCoroutine = StartCoroutine(TurnOnRoutine());
    }

    // 明滅を止めて消灯する
    public void TurnOff()
    {
        if (!isActiveAndEnabled) return;
        if (isDead) return;
        if (mainCoroutine != null) StopCoroutine(mainCoroutine);
        mainCoroutine = StartCoroutine(TurnOffRoutine());
    }

    // 1回限りの消灯演出
    // フェードイン → 安定点灯 → スタッター → 完全消灯(復帰なし)
    // TurnOnと併用せず、このメソッド単独でOnActivatedに接続する
    public void DieOut()
    {
        if (!isActiveAndEnabled) return;
        if (isDead) return;
        if (mainCoroutine != null) StopCoroutine(mainCoroutine);
        mainCoroutine = StartCoroutine(DieOutRoutine());
    }

    private IEnumerator TurnOnRoutine()
    {
        float elapsed = 0f;
        float start = light2D.intensity;
        while (elapsed < data.FadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / data.FadeInDuration);
            light2D.intensity = Mathf.Lerp(start, data.OnIntensity, t);
            yield return null;
        }
        light2D.intensity = data.OnIntensity;
        yield return FlickerLoop();
    }

    private IEnumerator TurnOffRoutine()
    {
        float elapsed = 0f;
        float start = light2D.intensity;
        while (elapsed < data.FadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / data.FadeOutDuration);
            light2D.intensity = Mathf.Lerp(start, 0f, t);
            yield return null;
        }
        light2D.intensity = 0f;
        mainCoroutine = null;
    }

    private IEnumerator DieOutRoutine()
    {
        // フェードイン
        float elapsed = 0f;
        float start = light2D.intensity;
        while (elapsed < data.FadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / data.FadeInDuration);
            light2D.intensity = Mathf.Lerp(start, data.OnIntensity, t);
            yield return null;
        }
        light2D.intensity = data.OnIntensity;

        // 安定点灯フェーズ(stableDurationで持続時間を制御)
        float stableDuration = Random.Range(data.StableDurationMin, data.StableDurationMax);
        float stableElapsed = 0f;
        while (stableElapsed < stableDuration)
        {
            stableElapsed += Time.deltaTime;
            float wobble = 1f - data.SubtleFlickerAmount * Mathf.PerlinNoise(Time.time * 10f, seed);
            light2D.intensity = data.OnIntensity * wobble;
            yield return null;
        }

        // スタッター(stutterCountで回数を制御)
        for (int i = 0; i < data.StutterCount; i++)
        {
            light2D.intensity = 0f;
            yield return new WaitForSeconds(data.StutterInterval);
            light2D.intensity = data.OnIntensity;
            yield return new WaitForSeconds(data.StutterInterval);
        }

        // 完全消灯、復帰なし
        light2D.intensity = 0f;
        isDead = true;
        mainCoroutine = null;
        onDied.Invoke();
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            float stableDuration = Random.Range(data.StableDurationMin, data.StableDurationMax);
            float stableElapsed = 0f;
            while (stableElapsed < stableDuration)
            {
                stableElapsed += Time.deltaTime;
                float wobble = 1f - data.SubtleFlickerAmount * Mathf.PerlinNoise(Time.time * 10f, seed);
                light2D.intensity = data.OnIntensity * wobble;
                yield return null;
            }

            for (int i = 0; i < data.StutterCount; i++)
            {
                light2D.intensity = 0f;
                yield return new WaitForSeconds(data.StutterInterval);
                light2D.intensity = data.OnIntensity;
                yield return new WaitForSeconds(data.StutterInterval);
            }

            light2D.intensity = 0f;
            float blackout = Random.Range(data.BlackoutDurationMin, data.BlackoutDurationMax);
            yield return new WaitForSeconds(blackout);
            light2D.intensity = data.OnIntensity;
        }
    }
}