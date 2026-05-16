using UnityEngine;
using UnityEngine.Events;

// 天井家具の着地後に通行不可エリアを生成し、イベント2終了時に消滅するバリア。
// CeilingObjectのbarrierObjectフィールドに設定することで着地後に自動的に有効化される。
[RequireComponent(typeof(Collider2D))]
public class BarrierObject : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("有効化時にゲームオブジェクトごと表示するか。Falseの場合はコライダーのみ有効化する")]
    [SerializeField] private bool showOnActivate = false;

    [Header("イベント")]
    [SerializeField] private UnityEvent onActivated;
    [SerializeField] private UnityEvent onDeactivated;

    private Collider2D barrierCollider;

    private void Awake()
    {
        barrierCollider = GetComponent<Collider2D>();

        // 初期状態：バリア無効
        SetBarrierActive(false);
    }

    // バリアを有効化する（CeilingObjectの着地後に呼び出される）
    public void Activate()
    {
        SetBarrierActive(true);
        onActivated?.Invoke();
    }

    // バリアを無効化する（イベント2終了時に呼び出す）
    public void Deactivate()
    {
        SetBarrierActive(false);
        onDeactivated?.Invoke();
    }

    // バリアの有効状態を切り替える内部処理
    private void SetBarrierActive(bool active)
    {
        barrierCollider.enabled = active;

        if (showOnActivate)
            gameObject.SetActive(active);
    }

    // 現在バリアが有効かどうかを取得する
    public bool IsActive => barrierCollider != null && barrierCollider.enabled;
}