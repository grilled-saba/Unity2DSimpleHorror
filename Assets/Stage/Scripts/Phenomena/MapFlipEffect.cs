using System.Collections;
using UnityEngine;

// マップ全体の反転演出を管理する
public class MapFlipEffect : MonoBehaviour
{
    [Header("反転設定")]
    [Tooltip("反転にかかる時間（秒）")]
    [Range(0.1f, 5f)]
    [SerializeField] private float flipDuration = 1.5f;

    [Tooltip("反転対象のMapRootオブジェクト")]
    [SerializeField] private Transform mapRoot;

    private bool isFlipped = false;
    private bool isAnimating = false;

    // 反転演出を開始する
    public void TriggerFlip()
    {
        if (isAnimating) return;
        StartCoroutine(FlipCoroutine());
    }

    // 指定時間をかけてZ軸180度回転させる
    private IEnumerator FlipCoroutine()
    {
        isAnimating = true;

        float elapsed = 0f;
        float startAngle = isFlipped ? 180f : 0f;
        float endAngle = isFlipped ? 0f : 180f;

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / flipDuration);

            // なめらかに加減速させる
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float currentAngle = Mathf.Lerp(startAngle, endAngle, smoothT);

            mapRoot.rotation = Quaternion.Euler(0f, 0f, currentAngle);

            yield return null;
        }

        mapRoot.rotation = Quaternion.Euler(0f, 0f, endAngle);
        isFlipped = !isFlipped;
        isAnimating = false;
    }
}