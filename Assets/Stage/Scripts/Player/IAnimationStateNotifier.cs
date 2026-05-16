using System;

// アニメーション状態を通知するインターフェース
public interface IAnimationStateNotifier
{
    event Action<bool> OnRunningChanged;
    event Action<bool> OnJumpingChanged;
}