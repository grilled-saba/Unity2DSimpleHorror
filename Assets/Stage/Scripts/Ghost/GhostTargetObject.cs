using System;
using UnityEngine;
using UnityEngine.Events;

// ゴーストの侵入対象オブジェクトに付けるコンポーネント
// (3 タンス / 9 ベッド / 20 蛇口 / 22 テレビ / 25 死体)
// 家具側のトリガーからActivate()を呼び、ゴーストの侵入・演出はイベントで受け取る
// 2回目の発動条件(床を踏む)はStepTriggerからNotifyPlayerApproach()を呼んで通知する
public class GhostTargetObject : MonoBehaviour
{
    [Header("侵入設定")]
    [Tooltip("ゴーストが侵入する位置。未指定なら自身の位置を使う")]
    [SerializeField] private Transform entryPoint;

    [Tooltip("前提となる対象。指定した場合、その対象が発動済みになるまでActivateを受け付けない(連鎖 19→20, 21→22 用)")]
    [SerializeField] private GhostTargetObject prerequisite;

    [Header("演出イベント")]
    [Tooltip("ゴーストが侵入し異常演出を開始するとき発火")]
    [SerializeField] private UnityEvent onAbnormalStarted;

    [Tooltip("ジャンプスケア発動時に発火(JumpScareEffect.Play等を接続)")]
    [SerializeField] private UnityEvent onJumpScare;

    [Tooltip("オブジェクト破損時に発火(SpriteSwapEffect等を接続)")]
    [SerializeField] private UnityEvent onBroken;

    // マネージャが購読する発動通知
    public event Action<GhostTargetObject> OnActivated;

    // ゴーストが購読するプレイヤー接近通知(潜伏後の2回目発動条件)
    public event Action OnPlayerApproached;

    public bool IsActivated { get; private set; }
    public bool IsCompleted { get; private set; }
    public Vector3 EntryPosition => entryPoint != null ? entryPoint.position : transform.position;

    // 家具側のトリガー・インタラクションから呼ぶ発動入口
    public void Activate()
    {
        if (IsActivated || IsCompleted) return;

        // 前提対象が未発動なら受け付けない
        if (prerequisite != null && !prerequisite.IsActivated) return;

        IsActivated = true;
        OnActivated?.Invoke(this);
    }

    // 床を踏むトリガー(StepTrigger)から呼ぶ接近通知
    // 発動用トリガーと同じものに接続してよい。ゴーストが潜伏後のみ反応する
    public void NotifyPlayerApproach()
    {
        OnPlayerApproached?.Invoke();
    }

    // 以下はGhostControllerから呼ばれる演出通知
    public void NotifyAbnormalStarted()
    {
        onAbnormalStarted.Invoke();
    }

    public void NotifyJumpScare()
    {
        onJumpScare.Invoke();
    }

    public void NotifyBroken()
    {
        IsCompleted = true;
        onBroken.Invoke();
    }
}