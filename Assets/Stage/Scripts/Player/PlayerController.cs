using System;
using UnityEngine;

// プレイヤーの移動とオブジェクト操作を管理する
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IInputStateReceiver, IAnimationStateNotifier, IFacingNotifier
{
    [Header("データ")]
    [SerializeField] private PlayerData playerData;

    [Header("アイテムを持つ位置")]
    [SerializeField] private Transform holdPoint;

    [Header("拾い判定")]
    [Tooltip("前方の拾い判定範囲。範囲内の対象のみ拾える")]
    [SerializeField] private PickupDetector pickupDetector;

    [Header("カメラ")]
    [Tooltip("マウス座標をワールド座標へ変換するためのカメラ")]
    [SerializeField] private Camera mainCamera;

    // テスト用のジャンプ機能。本番前にこのフィールドごと削除すること
    [Header("ジャンプ（テスト用）")]
    [Tooltip("ジャンプの強さ")]
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D rb;
    private PickableObject heldObject;
    private InputState inputState = InputState.Normal;
    private bool isRunning;
    private bool isJumping;
    // 現在右を向いているか
    private bool isFacingRight = true;

    // ItemTriggerが購読するアイテム操作イベント
    public event Action<PickableObject> OnItemPickedUp;
    public event Action OnItemDropped;

    // アニメーション状態の通知イベント
    public event Action<bool> OnRunningChanged;
    public event Action<bool> OnJumpingChanged;

    // 向きの変化を通知するイベント
    public event Action<bool> OnFacingChanged;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (inputState == InputState.Locked) return;

        HandleInteraction();

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

        // 実際の移動方向に合わせて向きを通知する
        if (input != 0f)
            NotifyFacing(input > 0f);
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

    // 向きが変化したときのみイベントを発行する
    private void NotifyFacing(bool value)
    {
        if (isFacingRight == value) return;
        isFacingRight = value;
        OnFacingChanged?.Invoke(isFacingRight);
    }

    // マウス左クリックで拾う・置くを切り替える
    // マウス左クリックで拾う・置くを切り替える
    private void HandleInteraction()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (heldObject == null)
            TryPickUp(worldPoint);
        else
            TryDrop(worldPoint);
    }

    // クリック地点の、範囲内オブジェクトを拾う
    private void TryPickUp(Vector2 worldPoint)
    {
        PickableObject pickable = pickupDetector.GetPickableAtPoint(worldPoint);
        if (pickable == null) return;

        heldObject = pickable;
        heldObject.PickUp(holdPoint);
        OnItemPickedUp?.Invoke(heldObject);
    }

    // 置く。クリック地点が範囲内のときのみ、その地点へ置く
    private void TryDrop(Vector2 worldPoint)
    {
        // クリック地点が範囲外なら何もしない（保持を維持）
        if (!pickupDetector.IsPointInRange(worldPoint)) return;

        // クリック地点に設置先があればそこへ設置する
        PlaceTarget target = pickupDetector.GetPlaceTargetAtPoint(worldPoint);
        if (target != null && target.TryPlace(heldObject.ItemId))
        {
            heldObject.Drop(target.transform.position);
            heldObject = null;
            OnItemDropped?.Invoke();
            return;
        }

        // クリック地点に置く
        heldObject.Drop(worldPoint);
        heldObject = null;
        OnItemDropped?.Invoke();
    }
}