using System;
using System.Collections;
using UnityEngine;

// ゴースト本体の状態マシン
// 出現 → 対象へ移動 → 侵入 → 異常演出 → 潜伏 → 床を踏む通知でジャンプスケア
// → 破損 → 再出現 → 消えて次の指示を待つ
// 対象の指示はGhostSequenceManagerから受け取る
[RequireComponent(typeof(SpriteRenderer))]
public class GhostController : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private GhostData ghostData;

    [Header("参照")]
    [Tooltip("出現位置。未指定なら現在位置から出現する")]
    [SerializeField] private Transform spawnPoint;

    // マネージャが購読する完了通知
    public event Action<GhostTargetObject> OnTargetCompleted;

    public bool IsBusy { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Coroutine sequenceCoroutine;
    private GhostTargetObject currentTarget;
    private bool approachReceived;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetAlpha(0f); // 初期は非表示
    }

    // マネージャから対象を割り当てられ、一連の流れを開始する
    public void AssignTarget(GhostTargetObject target)
    {
        if (IsBusy || target == null) return;
        IsBusy = true;
        currentTarget = target;
        sequenceCoroutine = StartCoroutine(GhostSequence(target));
    }

    private IEnumerator GhostSequence(GhostTargetObject target)
    {
        // 出現
        if (spawnPoint != null) transform.position = spawnPoint.position;
        yield return FadeTo(1f, ghostData.AppearDuration);

        // 対象へ移動(対象が途中で消えた場合は中断)
        while (target != null &&
               Vector2.Distance(transform.position, target.EntryPosition) > ghostData.ArriveDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, target.EntryPosition,
                ghostData.MoveSpeed * Time.deltaTime);
            yield return null;
        }

        if (target == null)
        {
            yield return AbortSequence();
            yield break;
        }

        // 侵入(姿を消す)して異常演出を開始
        yield return FadeTo(0f, ghostData.EnterDuration);
        target.NotifyAbnormalStarted();

        // 潜伏
        yield return new WaitForSeconds(ghostData.LurkDuration);

        // 潜伏後、床を踏む通知を待つ(潜伏中の通知は受け付けない)
        approachReceived = false;
        target.OnPlayerApproached += HandlePlayerApproached;
        yield return new WaitUntil(() => approachReceived || target == null);
        if (target != null) target.OnPlayerApproached -= HandlePlayerApproached;

        if (target == null)
        {
            yield return AbortSequence();
            yield break;
        }

        // ジャンプスケアと破損
        target.NotifyJumpScare();
        target.NotifyBroken();

        // 対象から再出現し、少し姿を見せてから消える
        transform.position = target.EntryPosition;
        yield return FadeTo(1f, ghostData.AppearDuration);
        yield return new WaitForSeconds(ghostData.ExitVisibleDuration);
        yield return FadeTo(0f, ghostData.EnterDuration);

        FinishSequence(target);
    }

    // 対象消失などで流れを中断し、非表示に戻す
    private IEnumerator AbortSequence()
    {
        yield return FadeTo(0f, ghostData.EnterDuration);
        FinishSequence(null);
    }

    // 一連の流れを終了し、マネージャへ通知する
    private void FinishSequence(GhostTargetObject completedTarget)
    {
        IsBusy = false;
        currentTarget = null;
        sequenceCoroutine = null;
        if (completedTarget != null) OnTargetCompleted?.Invoke(completedTarget);
    }

    private void HandlePlayerApproached()
    {
        approachReceived = true;
    }

    // 再生途中で無効化された場合の保険。購読と状態を確実に戻す
    private void OnDisable()
    {
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        if (currentTarget != null)
        {
            currentTarget.OnPlayerApproached -= HandlePlayerApproached;
            currentTarget = null;
        }

        IsBusy = false;
        SetAlpha(0f);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float start = spriteRenderer.color.a;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(start, targetAlpha, Mathf.Clamp01(elapsed / duration)));
            yield return null;
        }
        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float a)
    {
        Color c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;
    }
}