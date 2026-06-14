using System;

// プレイヤーの向きの変化を通知するインターフェース
public interface IFacingNotifier
{
    // 右を向いているときtrueを渡す。向きが変わったときのみ発行される
    event Action<bool> OnFacingChanged;
}