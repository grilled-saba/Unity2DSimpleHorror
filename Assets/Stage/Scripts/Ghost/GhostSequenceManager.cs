using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ゴースト対象の発動を追跡し、ゴーストへ順番に指示を出すマネージャ
// 複数の対象が発動された場合はキューに積み、ゴーストが空き次第 次の対象へ向かわせる
public class GhostSequenceManager : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("操作するゴースト")]
    [SerializeField] private GhostController ghost;

    [Tooltip("ステージ内の全侵入対象(5箇所)")]
    [SerializeField] private GhostTargetObject[] targets;

    [Header("イベント")]
    [Tooltip("全対象の破損が完了したとき発火")]
    [SerializeField] private UnityEvent onAllTargetsCompleted;

    private readonly Queue<GhostTargetObject> pendingTargets = new Queue<GhostTargetObject>();
    private int completedCount;
    private int totalTargets;

    private void Awake()
    {
        // 空のスロットを除いた実数を数える
        totalTargets = 0;
        foreach (var target in targets)
        {
            if (target != null) totalTargets++;
        }

        if (totalTargets == 0)
            Debug.LogWarning("[GhostSequenceManager] 侵入対象が設定されていません。");
    }

    private void OnEnable()
    {
        foreach (var target in targets)
        {
            if (target != null) target.OnActivated += HandleTargetActivated;
        }
        if (ghost != null) ghost.OnTargetCompleted += HandleTargetCompleted;
    }

    private void OnDisable()
    {
        foreach (var target in targets)
        {
            if (target != null) target.OnActivated -= HandleTargetActivated;
        }
        if (ghost != null) ghost.OnTargetCompleted -= HandleTargetCompleted;
    }

    // 対象が発動されたらキューに積み、ゴーストが空いていれば即向かわせる
    private void HandleTargetActivated(GhostTargetObject target)
    {
        pendingTargets.Enqueue(target);
        TryAssignNext();
    }

    // ゴーストが1件完了したら完了数を数え、次の対象へ
    private void HandleTargetCompleted(GhostTargetObject target)
    {
        completedCount++;

        if (completedCount >= totalTargets)
        {
            onAllTargetsCompleted.Invoke();
            return;
        }

        TryAssignNext();
    }

    private void TryAssignNext()
    {
        if (ghost == null || ghost.IsBusy) return;
        if (pendingTargets.Count == 0) return;
        ghost.AssignTarget(pendingTargets.Dequeue());
    }
}