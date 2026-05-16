using UnityEngine;

// 特定の床を踏んだときに発動するトリガー
// アタッチ先のオブジェクトにIs Trigger:falseのCollider2Dを付けて使用する
public class StepTrigger : BaseTrigger
{
    [Header("検出設定")]
    [Tooltip("検出対象のレイヤー（通常はPlayerレイヤー）")]
    [SerializeField] private LayerMask targetLayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsInTargetLayer(collision.gameObject)) return;
        Activate();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsInTargetLayer(collision.gameObject)) return;
        Deactivate();
    }

    private bool IsInTargetLayer(GameObject obj)
    {
        return (targetLayer.value & (1 << obj.layer)) != 0;
    }
}