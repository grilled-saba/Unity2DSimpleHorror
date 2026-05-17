using UnityEngine;
using UnityEngine.Events;

// 範囲攻撃（AOE）を発生させるコンポーネント。
// FloatingObjectやCeilingObjectにアタッチして使用する。
public class AoeEffect : MonoBehaviour
{
    [Header("AOE設定")]
    [Tooltip("AOEの範囲（半径）")]
    [Range(0.1f, 10f)]
    [SerializeField] private float aoeRadius = 2f;

    [Tooltip("AOEのダメージ量")]
    [Range(1, 100)]
    [SerializeField] private int aoeDamage = 20;

    [Tooltip("AOE攻撃の対象レイヤー")]
    [SerializeField] private LayerMask aoeTargetLayer;

    [Header("イベント")]
    [SerializeField] private UnityEvent onTriggered;

    // AOEを発動する
    public void Trigger()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            aoeRadius,
            aoeTargetLayer
        );

        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth == null) continue;

            playerHealth.TakeDamage(aoeDamage);
        }

        onTriggered?.Invoke();
    }

    // AOE範囲をエディタ上で可視化する
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}