using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// フルスクリーンの瞬間表示によるジャンプスケア(A-4)
// イベント1・Boss等で共通使用するコンポーネントとして設計
// 全画面画像はCanvasGroupのalphaで制御する
public class JumpScareEffect : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("全画面に表示する画像を持つCanvasGroup")]
    [SerializeField] private CanvasGroup scareCanvasGroup;

    [Header("表示設定")]
    [Tooltip("表示時間(秒)")]
    [Range(0.05f, 3f)]
    [SerializeField] private float duration = 0.5f;

    [Header("イベント")]
    [Tooltip("ジャンプスケア終了時に発火")]
    [SerializeField] private UnityEvent onFinished;

    private Coroutine playCoroutine;

    private void Awake()
    {
        if (scareCanvasGroup != null) scareCanvasGroup.alpha = 0f;
    }

    // ジャンプスケアを再生する
    public void Play()
    {
        if (!isActiveAndEnabled) return;
        if (playCoroutine != null) StopCoroutine(playCoroutine);
        playCoroutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (scareCanvasGroup != null) scareCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(duration);
        if (scareCanvasGroup != null) scareCanvasGroup.alpha = 0f;
        onFinished.Invoke();
        playCoroutine = null;
    }
}