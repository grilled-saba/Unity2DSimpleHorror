using System.Collections;
using UnityEngine;

// オブジェクトの浮遊・落下・AOE攻撃を管理する
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FloatingObject : MonoBehaviour
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
    }

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

    // 衝突時にAOEを発生させる
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;
        if (hasTriggeredAoe) return;

        hasTriggeredAoe = true;
        isActive = false;

        effect?.PlayLandEffect();
        TriggerAoe();
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