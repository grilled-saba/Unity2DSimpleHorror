using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 指定時間だけ光を明滅させ、終了後に元の明るさへ戻す
// (5 PC画面の明滅、21 テレビ台の光の明滅など、踏むたびに数秒明滅する用途)
// 消灯状態(Intensity 0)の光源にも使える。明滅時の明るさはこのコンポーネントで指定する
[RequireComponent(typeof(Light2D))]
public class TimedFlickerLight : MonoBehaviour
{
    [Header("明滅設定")]
    [Tooltip("明滅を続ける時間(秒)")]
    [Range(0.5f, 10f)]
    [SerializeField] private float flickerDuration = 3f;

    [Tooltip("点滅の切り替え間隔(秒)")]
    [Range(0.02f, 0.5f)]
    [SerializeField] private float flickerInterval = 0.08f;

    [Tooltip("明滅時の点灯の明るさ")]
    [Range(0f, 3f)]
    [SerializeField] private float onIntensity = 1.2f;

    private Light2D light2D;
    private Coroutine flickerCoroutine;
    private float originalIntensity;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        originalIntensity = light2D.intensity;
    }

    // 明滅を開始する(トリガーのOnActivatedに接続)
    public void Play()
    {
        if (!isActiveAndEnabled) return;
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        float elapsed = 0f;
        bool isOn = false;

        while (elapsed < flickerDuration)
        {
            isOn = !isOn;
            light2D.intensity = isOn ? onIntensity : 0f;
            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        // 元の明るさへ戻す
        light2D.intensity = originalIntensity;
        flickerCoroutine = null;
    }
}