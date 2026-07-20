using UnityEngine;

// 一度きりの発動状態を記録する小さなフラグ
// トリガーのUnityEventからMarkActivated()を呼び、
// GhostTargetObjectの連鎖条件(19→20, 21→22)の前提として参照する
public class ActivationFlag : MonoBehaviour
{
    public bool IsActivated { get; private set; }

    // 発動済みとして記録する(StepTrigger等のOnActivatedに接続)
    public void MarkActivated()
    {
        IsActivated = true;
    }

    // フラグを未発動へ戻す(テスト・リセット用)
    public void ClearFlag()
    {
        IsActivated = false;
    }
}