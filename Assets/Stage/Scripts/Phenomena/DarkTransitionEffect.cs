using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// 画面暗転演出(1C-1)
// アイテムを拾った直後に画面を暗転させ、短時間だけ入力をロックする
// 仕様: 暗転0.8〜1.0秒 / 入力ロック0.3秒 / 終了後即座に1C-3を発動
public class DarkTransitionEffect : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("全画面の黒画像を持つCanvasGroup")]
    [SerializeField] private CanvasGroup darkCanvasGroup;

    [Tooltip("入力状態を制御する対象(IInputStateReceiver実装)")]
    [SerializeField] private MonoBehaviour inputReceiverObject;

    [Header("時間設定")]
    [Tooltip("暗転にかける時間(秒)")]
    [Range(0.1f, 3f)]
    [SerializeField] private float fadeDuration = 0.9f;

    [Tooltip("完全に暗転した後、次の演出を発火するまでの待機時間(秒)")]
    [Range(0f, 5f)]
    [SerializeField] private float holdDuration = 0f;

    [Tooltip("入力をロックする時間(秒)")]
    [Range(0f, 2f)]
    [SerializeField] private float inputLockDuration = 0.3f;

    [Tooltip("暗転後に暗いまま保持するか(falseなら明転して戻す)")]
    [SerializeField] private bool keepDark = true;

    [Header("イベント")]
    [Tooltip("暗転・待機が完了したとき発火(1C-3 ScreenCompressのPlayなどに接続)")]
    [SerializeField] private UnityEvent onTransitionFinished;

    private IInputStateReceiver inputReceiver;
    private Coroutine routine;

    private void Awake()
    {
        inputReceiver = inputReceiverObject as IInputStateReceiver;

        if (inputReceiverObject != null && inputReceiver == null)
            Debug.LogWarning("[DarkTransitionEffect] inputReceiverObjectがIInputStateReceiverを実装していません。");

        if (darkCanvasGroup != null) darkCanvasGroup.alpha = 0f;
    }

    // 暗転を開始する
    public void Play()
    {
        if (!isActiveAndEnabled) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        inputReceiver?.SetInputState(InputState.Locked);
        bool inputUnlocked = false;

        // 暗転フェーズ
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            if (darkCanvasGroup != null) darkCanvasGroup.alpha = t;

            if (!inputUnlocked && elapsed >= inputLockDuration)
            {
                inputReceiver?.SetInputState(InputState.Normal);
                inputUnlocked = true;
            }
            yield return null;
        }

        if (darkCanvasGroup != null) darkCanvasGroup.alpha = 1f;
        if (!inputUnlocked) inputReceiver?.SetInputState(InputState.Normal);

        // 暗転状態を指定時間保持する
        if (holdDuration > 0f)
            yield return new WaitForSeconds(holdDuration);

        // 次の演出を発火
        onTransitionFinished.Invoke();

        if (!keepDark)
        {
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                if (darkCanvasGroup != null) darkCanvasGroup.alpha = 1f - t;
                yield return null;
            }
            if (darkCanvasGroup != null) darkCanvasGroup.alpha = 0f;
        }

        routine = null;
    }
}