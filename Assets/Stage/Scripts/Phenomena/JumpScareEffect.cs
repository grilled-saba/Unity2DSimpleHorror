using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// フルスクリーンでジャンプスケア画像を瞬間表示するコンポーネント。
// イベント1・Bossなど複数のイベントで共通使用する。
public class JumpScareEffect : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("表示するフルスクリーン画像オブジェクト")]
    [SerializeField] private GameObject jumpScareImage;

    [Header("表示設定")]
    [Tooltip("表示時間（秒）")]
    [Range(0.1f, 2f)]
    [SerializeField] private float displayDuration = 0.5f;

    [Header("イベント")]
    [SerializeField] private UnityEvent onStarted;
    [SerializeField] private UnityEvent onFinished;

    private bool isPlaying = false;

    private void Awake()
    {
        // 初期状態は非表示
        if (jumpScareImage != null)
            jumpScareImage.SetActive(false);
    }

    // ジャンプスケア演出を開始する（UnityEventまたは外部スクリプトから呼び出す）
    public void Trigger()
    {
        if (isPlaying) return;
        StartCoroutine(PlayCoroutine());
    }

    // 表示時間が経過したら自動的に非表示にするコルーチン
    private IEnumerator PlayCoroutine()
    {
        isPlaying = true;

        jumpScareImage.SetActive(true);
        onStarted?.Invoke();

        yield return new WaitForSeconds(displayDuration);

        jumpScareImage.SetActive(false);
        onFinished?.Invoke();

        isPlaying = false;
    }
}