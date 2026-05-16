using UnityEngine;

// 特定の範囲に侵入したときに発動するトリガー
// アタッチ先のオブジェクトにIs Trigger:trueのCollider2Dを付けて使用する
public class RangeTrigger : BaseTrigger
{
    [Header("検出設定")]
    [Tooltip("検出対象のレイヤー（通常はPlayerレイヤー）")]
    [SerializeField] private LayerMask targetLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInTargetLayer(other.gameObject)) return;
        Activate();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsInTargetLayer(other.gameObject)) return;
        Deactivate();
    }

    private bool IsInTargetLayer(GameObject obj)
    {
        return (targetLayer.value & (1 << obj.layer)) != 0;
    }
}