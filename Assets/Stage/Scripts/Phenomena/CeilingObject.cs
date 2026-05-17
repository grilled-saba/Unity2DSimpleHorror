using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// イベント2で天井に固定され、トリガー受信時に落下して通路を塞ぐ家具。
// 初期状態はRigidbody2DをStaticに設定して物理を無効化する。
// プレイヤーとの物理衝突はLayerCollisionMatrixで無効化しておくこと。
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CeilingObject : MonoBehaviour
{
    [Header("着地設定")]
    [Tooltip("着地とみなすレイヤー（通常はGroundレイヤー）")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("着地からバリア有効化までの待機時間（秒）")]
    [Range(0f, 2f)]
    [SerializeField] private float barrierActivateDelay = 0.3f;

    [Header("参照")]
    [Tooltip("着地後に有効化するバリアオブジェクト")]
    [SerializeField] private BarrierObject barrierObject;

    [Tooltip("着地時に発動するAOEコンポーネント。未設定の場合はAOEなし")]
    [SerializeField] private AoeEffect aoeEffect;

    [Header("イベント")]
    [SerializeField] private UnityEvent onDropStarted;
    [SerializeField] private UnityEvent onLanded;

    private Rigidbody2D rb;
    private bool hasDropped = false;
    private bool hasLanded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 初期状態：天井に固定、物理無効
        rb.bodyType = RigidbodyType2D.Static;
    }

    // 落下を開始する（UnityEventまたは外部スクリプトから呼び出す）
    public void TriggerDrop()
    {
        if (hasDropped) return;

        hasDropped = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        onDropStarted?.Invoke();
    }

    // Groundレイヤーとの衝突で着地を検出する
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasLanded) return;
        if ((groundLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        hasLanded = true;
        rb.bodyType = RigidbodyType2D.Static;

        aoeEffect?.Trigger();
        onLanded?.Invoke();

        if (barrierObject != null)
            StartCoroutine(ActivateBarrierAfterDelay());
    }

    // 遅延後にバリアを有効化するコルーチン
    private IEnumerator ActivateBarrierAfterDelay()
    {
        yield return new WaitForSeconds(barrierActivateDelay);
        barrierObject.Activate();
    }

    // 落下済みかどうかを取得する
    public bool HasDropped => hasDropped;

    // 着地済みかどうかを取得する
    public bool HasLanded => hasLanded;
}