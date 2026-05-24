using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Light2Dをホラー風にちらつかせる
[RequireComponent(typeof(Light2D))]
public class FlickerLight : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private LightingData lightingData;

    [Header("動作設定")]
    [Tooltip("起動時から自動でちらつくか")]
    [SerializeField] private bool flickerOnStart = false;

    [Tooltip("ちらつきパターンに揺らぎを加える")]
    [SerializeField] private bool useNoise = true;

    private Light2D light2D;
    private float baseIntensity;
    private Coroutine flickerCoroutine;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        baseIntensity = light2D.intensity;
    }

    private void Start()
    {
        if (flickerOnStart) StartFlicker();
    }

    // 外部からちらつきを開始する
    public void StartFlicker()
    {
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        flickerCoroutine = StartCoroutine(FlickerLoop());
    }

    // ちらつきを停止し基準値に戻す
    public void StopFlicker()
    {
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        flickerCoroutine = null;
        light2D.intensity = baseIntensity;
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            // ノイズを加える場合は乱数を緩やかにブレンド
            float ratio = useNoise
                ? Mathf.Lerp(
                    lightingData.FlickerMinRatio,
                    lightingData.FlickerMaxRatio,
                    Mathf.PerlinNoise(Time.time * 8f, 0f))
                : Random.Range(
                    lightingData.FlickerMinRatio,
                    lightingData.FlickerMaxRatio);

            light2D.intensity = baseIntensity * ratio;

            float wait = Random.Range(
                lightingData.FlickerIntervalMin,
                lightingData.FlickerIntervalMax);

            yield return new WaitForSeconds(wait);
        }
    }
}