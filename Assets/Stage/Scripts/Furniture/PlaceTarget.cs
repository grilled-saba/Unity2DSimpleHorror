using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// アイテムの設置先。正しいアイテムが設置されたとき発火する
[RequireComponent(typeof(Collider2D))]
public class PlaceTarget : MonoBehaviour, IInteractable
{
    [Header("設置条件")]
    [Tooltip("この設置先が受け入れるアイテムの識別子")]
    [SerializeField] private string acceptedItemId;

    [Header("イベント")]
    [Tooltip("正しいアイテムが設置されたとき発火")]
    [SerializeField] private UnityEvent onCorrectItemPlaced;
    [Tooltip("誤ったアイテムが設置されたとき発火")]
    [SerializeField] private UnityEvent onWrongItemPlaced;

    private bool isFilled;
    private PlaceInteraction placeInteraction;
    private readonly List<IInteraction> availableInteractions = new List<IInteraction>(1);

    public bool IsFilled => isFilled;
    public string AcceptedItemId => acceptedItemId;

    private void Awake()
    {
        placeInteraction = new PlaceInteraction(this);
    }

    // 設置を試みる。識別子が一致すれば成功しtrueを返す
    public bool TryPlace(string itemId)
    {
        if (isFilled) return false;
        if (itemId == acceptedItemId)
        {
            isFilled = true;
            onCorrectItemPlaced.Invoke();
            return true;
        }
        onWrongItemPlaced.Invoke();
        return false;
    }

    // まだ埋まっておらず、持っているアイテムの識別子が一致するときのみ「置く」を返す
    public IReadOnlyList<IInteraction> GetAvailableInteractions(IItemHolder holder)
    {
        availableInteractions.Clear();
        if (!isFilled && holder != null && holder.IsHolding && holder.HeldItemId == acceptedItemId)
        {
            availableInteractions.Add(placeInteraction);
        }
        return availableInteractions;
    }
}

// 「置く」相互作用
public class PlaceInteraction : IInteraction
{
    private readonly PlaceTarget target;

    public PlaceInteraction(PlaceTarget target)
    {
        this.target = target;
    }

    public InteractionType Type => InteractionType.Place;

    public void Execute(IItemHolder holder)
    {
        holder.PlaceHeldItemOn(target);
    }
}