using UnityEngine;

// プレイヤーと衝突しない重い家具
// 地面との衝突時のみAOEを発動する
public class HeavyFloatingObject : BaseFloatingObject
{
    // 地面との衝突時のみAOEを発動する
    protected override bool ShouldTriggerAoe(Collision2D collision)
    {
        return collision.gameObject.CompareTag("Ground");
    }
}