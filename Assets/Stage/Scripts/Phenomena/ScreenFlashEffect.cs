using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// 画面全体を数回明滅させる演出(12 物置のドアの画面明滅など)
// 全画面画像のCanvasGroupを点滅させ、終了後にイベントを発火する
public class ScreenFlashEffect : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("全画面画像を持つCanvasGroup(黒画像などを想定)")]
    [SerializeField] private CanvasGroup flashCanvasGroup;

    [Header("明滅設定")]
    [Tooltip("明滅の回数")]
    [Range(1, 10)]
    [SerializeField] private int flashCount = 3;

    [Tooltip("明滅1回あたりの間隔(秒)")]
    [Range(0.02f, 0.5f)]
    [SerializeField] private float flashInterval = 0.08f;

    [Header("イベント")]
    [Tooltip("明滅が終わったとき発火(シーン転換などを接続)")]
    [SerializeField] private UnityEvent onFinished;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (flashCanvasGroup != null) flashCanvasGroup.alpha = 0f;
    }

    // 明滅を開始する
    public void Play()
    {
        if (!isActiveAndEnabled) return;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            if (flashCanvasGroup != null) flashCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(flashInterval);
            if (flashCanvasGroup != null) flashCanvasGroup.alpha = 0f;
            yield return new WaitForSeconds(flashInterval);
        }

        flashCoroutine = null;
        onFinished.Invoke();
    }
}