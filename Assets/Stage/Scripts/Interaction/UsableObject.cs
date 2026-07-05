using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ポップアップの「使う」ボタンで任意の動作を発動させるオブジェクトに付けるコンポーネント
// (例: 物置のドア — ボタンを押すと画面明滅ののちシーン転換)
[RequireComponent(typeof(Collider2D))]
public class UsableObject : MonoBehaviour, IInteractable
{
    [Header("使用設定")]
    [Tooltip("OFFにすると使用不可になる")]
    [SerializeField] private bool isUsable = true;

    [Header("イベント")]
    [Tooltip("使うボタンが押されたとき発火")]
    [SerializeField] private UnityEvent onUsed;

    private UseInteraction useInteraction;
    // GetAvailableInteractionsで返す使い回しリスト
    private readonly List<IInteraction> availableInteractions = new List<IInteraction>(1);

    private void Awake()
    {
        useInteraction = new UseInteraction(this);
    }

    // 使用可否を外部から切り替える(イベント進行での表示制御用)
    public void SetUsable(bool value)
    {
        isUsable = value;
    }

    // ボタンが押されたときにUseInteractionから呼ばれる
    public void Use()
    {
        onUsed.Invoke();
    }

    public IReadOnlyList<IInteraction> GetAvailableInteractions(IItemHolder holder)
    {
        availableInteractions.Clear();
        if (isUsable)
        {
            availableInteractions.Add(useInteraction);
        }
        return availableInteractions;
    }
}

// 「使う」相互作用
public class UseInteraction : IInteraction
{
    private readonly UsableObject target;

    public UseInteraction(UsableObject target)
    {
        this.target = target;
    }

    public InteractionType Type => InteractionType.Use;

    public void Execute(IItemHolder holder)
    {
        target.Use();
    }
}