using System.Collections;
using UnityEngine;

// オブジェクトを一定時間振動させる汎用コンポーネント
public class VibrateEffect : MonoBehaviour
{
    [Header("振動設定")]
    [Tooltip("振動の振幅(ユニット)")]
    [SerializeField] private Vector2 amplitude = new Vector2(0.1f, 0.1f);

    [Tooltip("振動継続時間(秒)")]
    [Range(0.1f, 10f)]
    [SerializeField] private float duration = 1.5f;

    [Tooltip("1秒あたりの振動回数")]
    [Range(1f, 60f)]
    [SerializeField] private float frequency = 25f;

    [Tooltip("終了時に元の位置へ戻すか")]
    [SerializeField] private bool returnToOrigin = true;

    private Vector3 originPosition;
    private Coroutine vibrateCoroutine;

    private void Awake()
    {
        originPosition = transform.localPosition;
    }

    // 外部から振動を開始する
    public void Play()
    {
        if (!isActiveAndEnabled) return;
        if (vibrateCoroutine != null) StopCoroutine(vibrateCoroutine);
        vibrateCoroutine = StartCoroutine(VibrateRoutine());
    }

    // 振動を即時停止する
    public void Stop()
    {
        if (vibrateCoroutine != null) StopCoroutine(vibrateCoroutine);
        vibrateCoroutine = null;
        if (returnToOrigin) transform.localPosition = originPosition;
    }

    private IEnumerator VibrateRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // sin/cosで上下左右に滑らかに揺らす
            float phase = elapsed * frequency * Mathf.PI * 2f;
            float offsetX = amplitude.x * Mathf.Sin(phase);
            float offsetY = amplitude.y * Mathf.Cos(phase);
            transform.localPosition = originPosition + new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }

        if (returnToOrigin) transform.localPosition = originPosition;
        vibrateCoroutine = null;
    }
}