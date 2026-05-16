using System;
using UnityEngine;

// プレイヤーの移動とオブジェクト操作を管理する
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IInputStateReceiver, IAnimationStateNotifier
{
    [Header("データ")]
    [SerializeField] private PlayerData playerData;

    [Header("アイテムを持つ位置")]
    [SerializeField] private Transform holdPoint;

    // テスト用のジャンプ機能。本番前にこのフィールドごと削除すること
    [Header("ジャンプ（テスト用）")]
    [Tooltip("ジャンプの強さ")]
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D rb;
    private PickableObject heldObject;
    private InputState inputState = InputState.Normal;

    private bool isRunning;
    private bool isJumping;

    // ItemTriggerが購読するアイテム操作イベント
    public event Action<PickableObject> OnItemPickedUp;
    public event Action OnItemDropped;

    // アニメーション状態の通知イベント
    public event Action<bool> OnRunningChanged;
    public event Action<bool> OnJumpingChanged;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (inputState == InputState.Locked) return;

        HandlePickUp();
        HandleDrop();

        // テスト用のジャンプ処理。本番前にこのブロックごと削除すること
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            NotifyJumping(true);
        }

        // 着地の検知
        if (isJumping && rb.linearVelocity.y == 0f)
            NotifyJumping(false);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // 外部から入力状態を設定する
    public void SetInputState(InputState state)
    {
        inputState = state;
    }

    // 左右移動の処理
    private void HandleMovement()
    {
        if (inputState == InputState.Locked)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            NotifyRunning(false);
            return;
        }

        float input = 0f;
        if (Input.GetKey(KeyCode.A)) input = -1f;
        if (Input.GetKey(KeyCode.D)) input = 1f;

        // 入力反転状態では左右を逆にする
        if (inputState == InputState.Inverted) input = -input;

        rb.linearVelocity = new Vector2(input * playerData.MoveSpeed, rb.linearVelocity.y);

        NotifyRunning(input != 0f);
    }

    // 走り状態が変化したときのみイベントを発行する
    private void NotifyRunning(bool value)
    {
        if (isRunning == value) return;
        isRunning = value;
        OnRunningChanged?.Invoke(isRunning);
    }

    // ジャンプ状態が変化したときのみイベントを発行する
    private void NotifyJumping(bool value)
    {
        if (isJumping == value) return;
        isJumping = value;
        OnJumpingChanged?.Invoke(isJumping);
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
            if (!pickable.IsPickable) continue;

            heldObject = pickable;
            heldObject.PickUp(holdPoint);
            OnItemPickedUp?.Invoke(heldObject);
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
        OnItemDropped?.Invoke();
    }
}