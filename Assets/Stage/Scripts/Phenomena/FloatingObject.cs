using UnityEngine;

// プレイヤーと衝突する浮遊家具
// あらゆるオブジェクトとの衝突でAOEを発動する
public class FloatingObject : BaseFloatingObject
{
    // ShouldTriggerAoe のオーバーライドなし
    // 基底クラスの実装（常にtrue）をそのまま使用する
}