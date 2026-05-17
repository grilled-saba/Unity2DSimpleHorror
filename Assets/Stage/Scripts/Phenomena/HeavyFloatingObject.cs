using UnityEngine;

// プレイヤーと衝突しない重い家具。
// 地面レイヤーとの衝突時のみAOEを発動する。
public class HeavyFloatingObject : BaseFloatingObject
{
    [Header("着地判定")]
    [Tooltip("着地とみなすレイヤー（通常はGroundレイヤー）")]
    [SerializeField] private LayerMask groundLayer;

    // 地面レイヤーとの衝突時のみAOEを発動する
    protected override bool ShouldTriggerAoe(Collision2D collision)
    {
        return (groundLayer.value & (1 << collision.gameObject.layer)) != 0;
    }
}