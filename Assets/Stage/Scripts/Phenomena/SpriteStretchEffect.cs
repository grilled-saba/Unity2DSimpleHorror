using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// 指定タグの付いたオブジェクト全体のスプライトを横に引き伸ばす異常現象(ステージ3用)
// 部屋中の家具が同時に伸びることで、目の錯覚が起きているような雰囲気を作る
// 再生中はプレイヤーの移動とクリック操作の両方をロックし、終了後に解除する
public class SpriteStretchEffect : MonoBehaviour
{
    [Header("対象設定")]
    [Tooltip("引き伸ばす対象のタグ。Tag Managerに登録したタグ名を入力する")]
    [SerializeField] private string targetTag = "Stretchable";

    [Header("伸び設定")]
    [Tooltip("X軸の最大倍率(元の大きさに対する倍率)")]
    [Range(1.1f, 10f)]
    [SerializeField] private float targetScaleX = 3f;

    [Tooltip("最大まで伸びるのにかける時間(秒)")]
    [Range(0.2f, 10f)]
    [SerializeField] private float stretchDuration = 2f;

    [Tooltip("伸びきった状態を保つ時間(秒)。0で即座に戻る")]
    [Range(0f, 3f)]
    [SerializeField] private float holdDuration = 0.2f;

    [Header("入力ロック")]
    [Tooltip("移動入力を制御する対象(IInputStateReceiver実装)")]
    [SerializeField] private MonoBehaviour inputReceiverObject;

    [Tooltip("クリック操作を遮断するためのCanvasGroup(ポップアップUIのWorld Spaceキャンバスに付ける)")]
    [SerializeField] private CanvasGroup interactionCanvasGroup;

    [Header("イベント")]
    [Tooltip("元の大きさへ戻ったとき発火")]
    [SerializeField] private UnityEvent onFinished;

    private IInputStateReceiver inputReceiver;
    private Transform[] targets;
    private Vector3[] originalScales;
    private Coroutine stretchCoroutine;
    private bool isLocking;

    private void Awake()
    {
        inputReceiver = inputReceiverObject as IInputStateReceiver;

        if (inputReceiverObject != null && inputReceiver == null)
            Debug.LogWarning("[SpriteStretchEffect] inputReceiverObjectがIInputStateReceiverを実装していません。");
    }

    // 伸び演出を再生する(アイテム拾得トリガーなどに接続)
    public void Play()
    {
        if (!isActiveAndEnabled) return;

        // 再生中の多重発動は無視する(グループ演出の破綻を防ぐ)
        if (stretchCoroutine != null) return;

        CollectTargets();
        if (targets == null || targets.Length == 0)
        {
            Debug.LogWarning($"[SpriteStretchEffect] タグ '{targetTag}' のオブジェクトが見つかりません。");
            return;
        }

        stretchCoroutine = StartCoroutine(StretchRoutine());
    }

    // タグの付いたオブジェクトを集め、元のスケールを記録する
    private void CollectTargets()
    {
        GameObject[] found;
        try
        {
            found = GameObject.FindGameObjectsWithTag(targetTag);
        }
        catch (UnityException)
        {
            // タグ自体が未登録の場合
            Debug.LogWarning($"[SpriteStretchEffect] タグ '{targetTag}' がTag Managerに登録されていません。");
            targets = null;
            return;
        }

        targets = new Transform[found.Length];
        originalScales = new Vector3[found.Length];

        for (int i = 0; i < found.Length; i++)
        {
            targets[i] = found[i].transform;
            originalScales[i] = found[i].transform.localScale;
        }
    }

    private IEnumerator StretchRoutine()
    {
        LockInput();

        // ゆっくり伸びる
        float elapsed = 0f;
        while (elapsed < stretchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / stretchDuration);
            // 緩急のあるイージングで飴のような伸び方にする
            float eased = t * t * (3f - 2f * t);

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] == null) continue;
                Vector3 scale = originalScales[i];
                scale.x = Mathf.Lerp(originalScales[i].x, originalScales[i].x * targetScaleX, eased);
                targets[i].localScale = scale;
            }
            yield return null;
        }

        // 伸びきった状態を保持する
        if (holdDuration > 0f)
            yield return new WaitForSeconds(holdDuration);

        // 一瞬で元へ戻す
        RestoreScales();

        UnlockInput();
        stretchCoroutine = null;
        onFinished.Invoke();
    }

    // 全対象を元のスケールへ戻す
    private void RestoreScales()
    {
        if (targets == null) return;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;
            targets[i].localScale = originalScales[i];
        }
    }

    // 移動とクリックの両方をロックする
    private void LockInput()
    {
        isLocking = true;
        inputReceiver?.SetInputState(InputState.Locked);

        if (interactionCanvasGroup != null)
        {
            interactionCanvasGroup.interactable = false;
            interactionCanvasGroup.blocksRaycasts = false;
        }
    }

    // ロックを解除する
    private void UnlockInput()
    {
        if (!isLocking) return;
        isLocking = false;

        inputReceiver?.SetInputState(InputState.Normal);

        if (interactionCanvasGroup != null)
        {
            interactionCanvasGroup.interactable = true;
            interactionCanvasGroup.blocksRaycasts = true;
        }
    }

    // 再生途中で無効化された場合の保険。スケールとロックを元に戻す
    private void OnDisable()
    {
        if (stretchCoroutine != null)
        {
            StopCoroutine(stretchCoroutine);
            stretchCoroutine = null;
            RestoreScales();
            UnlockInput();
        }
    }
}