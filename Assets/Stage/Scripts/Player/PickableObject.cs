using System.Collections.Generic;
using UnityEngine;

// 拾えるオブジェクトに付けるコンポーネント
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PickableObject : MonoBehaviour, IInteractable
{
    [Header("持ち上げ設定")]
    [Tooltip("OFFにすると持ち上げ不可になる")]
    [SerializeField] private bool isPickable = true;

    [Header("識別")]
    [Tooltip("設置先(PlaceTarget)との照合に使うアイテム識別子")]
    [SerializeField] private string itemId;

    public bool IsPickable => isPickable;
    public string ItemId => itemId;

    private Rigidbody2D rb;
    private PickUpInteraction pickUpInteraction;
    // GetAvailableInteractionsで返す使い回しリスト
    private readonly List<IInteraction> availableInteractions = new List<IInteraction>(1);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pickUpInteraction = new PickUpInteraction(this);
    }

    // 拾い上げ処理
    public void PickUp(Transform holdPoint)
    {
        rb.simulated = false;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
    }

    // 指定位置に置く処理
    public void Drop(Vector2 position)
    {
        transform.SetParent(null);
        transform.position = position;
        rb.simulated = true;
    }

    // 設置先に固定する。以降は拾えなくなる
    public void PlaceAndLock(Vector2 position)
    {
        transform.SetParent(null);
        transform.position = position;
        rb.simulated = false;
        isPickable = false;
    }

    // 持ち上げ可能で、プレイヤーが何も持っていないときのみ「持ち上げ」を返す
    public IReadOnlyList<IInteraction> GetAvailableInteractions(IItemHolder holder)
    {
        availableInteractions.Clear();
        if (isPickable && holder != null && !holder.IsHolding)
        {
            availableInteractions.Add(pickUpInteraction);
        }
        return availableInteractions;
    }
}

// 「持ち上げ」相互作用
public class PickUpInteraction : IInteraction
{
    private readonly PickableObject target;

    public PickUpInteraction(PickableObject target)
    {
        this.target = target;
    }

    public InteractionType Type => InteractionType.PickUp;

    public void Execute(IItemHolder holder)
    {
        holder.Hold(target);
    }
}