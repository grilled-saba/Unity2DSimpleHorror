using System;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーの移動とオブジェクト操作を管理する
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IInputStateReceiver, IAnimationStateNotifier, IFacingNotifier, IItemHolder
{
    [Header("データ")]
    [SerializeField] private PlayerData playerData;

    [Header("アイテムを持つ位置")]
    [SerializeField] private Transform holdPoint;

    [Header("相互作用判定")]
    [Tooltip("前方の相互作用判定範囲")]
    [SerializeField] private InteractionDetector interactionDetector;

    // テスト用のジャンプ機能。本番前にこのフィールドごと削除すること
    [Header("ジャンプ（テスト用）")]
    [Tooltip("ジャンプの強さ")]
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D rb;
    private PickableObject heldObject;
    private InputState inputState = InputState.Normal;
    private bool isRunning;
    private bool isJumping;
    private bool isFacingRight = true;

    public event Action<PickableObject> OnItemPickedUp;
    public event Action OnItemDropped;
    public event Action<bool> OnRunningChanged;
    public event Action<bool> OnJumpingChanged;
    public event Action<bool> OnFacingChanged;

    // 現在アイテムを持っているか
    public bool IsHolding => heldObject != null;

    // 持っているアイテムの識別子。持っていないときはnull
    public string HeldItemId => heldObject != null ? heldObject.ItemId : null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (inputState == InputState.Locked) return;

        // テスト用のジャンプ処理。本番前にこのブロックごと削除すること
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            NotifyJumping(true);
        }

        if (isJumping && rb.linearVelocity.y == 0f)
            NotifyJumping(false);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public void SetInputState(InputState state)
    {
        inputState = state;
    }

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

        if (inputState == InputState.Inverted) input = -input;

        rb.linearVelocity = new Vector2(input * playerData.MoveSpeed, rb.linearVelocity.y);
        NotifyRunning(input != 0f);

        if (input != 0f)
            NotifyFacing(input > 0f);
    }

    private void NotifyRunning(bool value)
    {
        if (isRunning == value) return;
        isRunning = value;
        OnRunningChanged?.Invoke(isRunning);
    }

    private void NotifyJumping(bool value)
    {
        if (isJumping == value) return;
        isJumping = value;
        OnJumpingChanged?.Invoke(isJumping);
    }

    private void NotifyFacing(bool value)
    {
        if (isFacingRight == value) return;
        isFacingRight = value;
        OnFacingChanged?.Invoke(isFacingRight);
    }

    // アイテムを持ち上げる
    public void Hold(PickableObject pickable)
    {
        if (heldObject != null) return;
        heldObject = pickable;
        heldObject.PickUp(holdPoint);
        OnItemPickedUp?.Invoke(heldObject);
    }

    // 持っているアイテムを設置先へ置く。正しければ固定し、誤りなら手持ちのまま
    public void PlaceHeldItemOn(PlaceTarget target)
    {
        if (heldObject == null) return;
        if (target.TryPlace(heldObject.ItemId))
        {
            heldObject.PlaceAndLock(target.transform.position);
            heldObject = null;
            OnItemDropped?.Invoke();
        }
        // 失敗時はTryPlace内でonWrongItemPlacedが発火。手持ちは維持する
    }

    // テスト用。最寄りの実行可能な相互作用を実行する。UI実装後にこのメソッドごと削除すること
    private void HandleTestInteraction()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        foreach (IInteractable interactable in interactionDetector.Interactables)
        {
            IReadOnlyList<IInteraction> options = interactable.GetAvailableInteractions(this);
            if (options.Count > 0)
            {
                options[0].Execute(this);
                return;
            }
        }
    }
}