using System.Collections;
using UnityEngine;

// 浮遊家具の共通処理を定義する基底クラス
// FloatingObject・HeavyFloatingObject はこのクラスを継承する
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class BaseFloatingObject : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private FloatingObjectData floatingData;

    [Header("エフェクト")]
    [SerializeField] private FloatingObjectEffect effect;

    [Tooltip("AOE攻撃の対象レイヤー")]
    [SerializeField] private LayerMask aoeTargetLayer;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isActive = false;
    private bool hasTriggeredAoe = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        OnAwake();
    }

    // サブクラスで追加の初期化が必要な場合にオーバーライドする
    protected virtual void OnAwake() { }

    // 浮遊演出を外部から起動する
    public void TriggerFloat()
    {
        if (isActive) return;
        StartCoroutine(FloatSequence());
    }

    // 浮遊から落下までの一連の流れ
    private IEnumerator FloatSequence()
    {
        isActive = true;
        hasTriggeredAoe = false;

        rb.simulated = false;
        effect?.PlayFloatEffect();

        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + Vector2.up * floatingData.FloatHeight;
        float elapsed = 0f;

        // 指定時間をかけて上昇する
        while (elapsed < floatingData.FloatDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / floatingData.FloatDuration);
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // 浮遊状態を一定時間維持する
        yield return new WaitForSeconds(floatingData.FloatHoldDuration);

        effect?.StopFloatEffect();

        // 物理を再び有効にして落下させる
        rb.simulated = true;

        // 3秒以内に衝突がなければ非アクティブに戻す
        yield return new WaitForSeconds(3f);
        isActive = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;
        if (hasTriggeredAoe) return;
        if (!ShouldTriggerAoe(collision)) return;

        hasTriggeredAoe = true;
        isActive = false;

        effect?.PlayLandEffect();
        TriggerAoe();
    }

    // AOEを発動すべき衝突かを判定する
    // 基底実装は常にtrueを返す。サブクラスで条件を上書きできる
    protected virtual bool ShouldTriggerAoe(Collision2D collision)
    {
        return true;
    }

    // オブジェクトサイズに基づいてAOE攻撃を発生させる
    private void TriggerAoe()
    {
        float aoeRadius = col.bounds.extents.x * floatingData.AoeRadiusMultiplier;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            aoeRadius,
            aoeTargetLayer
        );

        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth == null) continue;

            playerHealth.TakeDamage(floatingData.AoeDamage);
        }
    }

    // AOE範囲をエディタ上で可視化する
    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<Collider2D>();
        if (floatingData == null) return;

        Gizmos.color = Color.red;
        float aoeRadius = col.bounds.extents.x * floatingData.AoeRadiusMultiplier;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}