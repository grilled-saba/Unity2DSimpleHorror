using UnityEngine;

// プレイヤーの移動とオブジェクト操作を管理する
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private PlayerData playerData;

    [Header("アイテムを持つ位置")]
    [SerializeField] private Transform holdPoint;

    private Rigidbody2D rb;
    private PickableObject heldObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandlePickUp();
        HandleDrop();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // 左右移動の処理
    private void HandleMovement()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A)) input = -1f;
        if (Input.GetKey(KeyCode.D)) input = 1f;

        rb.linearVelocity = new Vector2(input * playerData.MoveSpeed, rb.linearVelocity.y);
    }

    // Wキーで近くのオブジェクトを拾う
    private void HandlePickUp()
    {
        if (!Input.GetKeyDown(KeyCode.W)) return;
        if (heldObject != null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            playerData.PickUpRadius
        );

        foreach (Collider2D hit in hits)
        {
            PickableObject pickable = hit.GetComponent<PickableObject>();
            if (pickable == null) continue;

            heldObject = pickable;
            heldObject.PickUp(holdPoint);
            break;
        }
    }

    // Sキーで足元にオブジェクトを置く
    private void HandleDrop()
    {
        if (!Input.GetKeyDown(KeyCode.S)) return;
        if (heldObject == null) return;

        Vector2 dropPosition = new Vector2(
            transform.position.x,
            transform.position.y - 1f
        );

        heldObject.Drop(dropPosition);
        heldObject = null;
    }
}