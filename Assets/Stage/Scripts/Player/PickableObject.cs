using UnityEngine;

// 拾えるオブジェクトに付けるコンポーネント
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PickableObject : MonoBehaviour
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
}