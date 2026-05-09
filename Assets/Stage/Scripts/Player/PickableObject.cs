using UnityEngine;

// 拾えるオブジェクトに付けるコンポーネント
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PickableObject : MonoBehaviour
{
    [Header("持ち上げ設定")]
    [Tooltip("OFFにすると持ち上げ不可になる")]
    [SerializeField] private bool isPickable = true;

    public bool IsPickable => isPickable;

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