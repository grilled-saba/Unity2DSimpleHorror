using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// イベント2で天井に固定され、トリガー受信時に落下して通路を塞ぐ家具。
// 初期状態はRigidbody2DをStaticに設定して物理を無効化する。
// MapFlipEffectはカメラのみを操作するため、落下前の位置はマップ回転の影響を受けない。
[RequireComponent(typeof(Rigidbody2D))]
public class CeilingObject : MonoBehaviour
{
    [Header("落下設定")]
    [Tooltip("着地判定に使う速度のしきい値（この値以下になったら着地とみなす）")]
    [Range(0.01f, 0.5f)]
    [SerializeField] private float landingVelocityThreshold = 0.05f;

    [Tooltip("速度監視を開始するまでの待機時間（秒）。落下直後のゼロ速度を誤検知しないための猶予時間")]
    [Range(0.1f, 1f)]
    [SerializeField] private float landingCheckDelay = 0.2f;

    [Tooltip("着地からバリア有効化までの待機時間（秒）")]
    [Range(0f, 2f)]
    [SerializeField] private float barrierActivateDelay = 0.3f;

    [Header("参照")]
    [Tooltip("着地後に有効化するバリアオブジェクト。未設定の場合はこのオブジェクト自体がバリアとして機能する")]
    [SerializeField] private BarrierObject barrierObject;

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

    // 落下を開始する（UnityEvent、または外部スクリプトから呼び出す）
    public void TriggerDrop()
    {
        if (hasDropped) return;

        hasDropped = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        onDropStarted?.Invoke();
        StartCoroutine(WaitForLanding());
    }

    // 速度が一定以下になったことで着地を検出するコルーチン
    private IEnumerator WaitForLanding()
    {
        // 落下直後は速度が0に近いため、監視開始を少し遅らせる
        yield return new WaitForSeconds(landingCheckDelay);

        while (!hasLanded)
        {
            // Unity 2022以降では rb.velocity が非推奨のため rb.linearVelocity を使用すること
            if (Mathf.Abs(rb.linearVelocity.y) < landingVelocityThreshold)
            {
                OnLanded();
            }
            yield return null;
        }
    }

    // 着地時の処理
    private void OnLanded()
    {
        hasLanded = true;

        // 着地後は動かないようにStaticに戻す
        rb.bodyType = RigidbodyType2D.Static;

        onLanded?.Invoke();
        StartCoroutine(ActivateBarrierAfterDelay());
    }

    // 遅延後にバリアを有効化するコルーチン
    private IEnumerator ActivateBarrierAfterDelay()
    {
        yield return new WaitForSeconds(barrierActivateDelay);
        barrierObject?.Activate();
    }

    // 落下済みかどうかを取得する
    public bool HasDropped => hasDropped;

    // 着地済みかどうかを取得する
    public bool HasLanded => hasLanded;
}