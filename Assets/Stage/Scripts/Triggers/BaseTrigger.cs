using System;
using UnityEngine;
using UnityEngine.Events;

// 全トリガーの基底クラス
// サブクラスはActivate/Deactivateを呼び出してトリガー状態を管理する
public abstract class BaseTrigger : MonoBehaviour
{
    [Header("トリガーイベント")]
    [Tooltip("トリガーが有効になったときに呼ばれるイベント")]
    [SerializeField] private UnityEvent onActivated;

    [Tooltip("トリガーが無効になったときに呼ばれるイベント")]
    [SerializeField] private UnityEvent onDeactivated;

    // CompositeTriggerがコードから購読するためのC#イベント
    public event Action OnActivated;
    public event Action OnDeactivated;

    public bool IsActivated { get; private set; }

    // トリガーを有効にしてイベントを発行する
    protected void Activate()
    {
        if (IsActivated) return;
        IsActivated = true;
        OnActivated?.Invoke();
        onActivated?.Invoke();
    }

    // トリガーを無効にしてイベントを発行する
    protected void Deactivate()
    {
        if (!IsActivated) return;
        IsActivated = false;
        OnDeactivated?.Invoke();
        onDeactivated?.Invoke();
    }
}