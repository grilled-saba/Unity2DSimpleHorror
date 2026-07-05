using UnityEngine;

// ゴースト演出に関わる数値を一括管理するScriptableObject
[CreateAssetMenu(fileName = "GhostData", menuName = "9T/Ghost/GhostData")]
public class GhostData : ScriptableObject
{
    [Header("移動")]
    [Tooltip("対象オブジェクトへ向かう移動速度")]
    [Range(0.5f, 10f)]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("対象に到達したとみなす距離")]
    [Range(0.05f, 1f)]
    [SerializeField] private float arriveDistance = 0.2f;

    [Header("出現・消失")]
    [Tooltip("出現時のフェードイン時間(秒)")]
    [Range(0.1f, 3f)]
    [SerializeField] private float appearDuration = 0.6f;

    [Tooltip("オブジェクト侵入時のフェードアウト時間(秒)")]
    [Range(0.1f, 3f)]
    [SerializeField] private float enterDuration = 0.5f;

    [Tooltip("破損後に姿を見せている時間(秒)")]
    [Range(0f, 5f)]
    [SerializeField] private float exitVisibleDuration = 1f;

    [Header("潜伏")]
    [Tooltip("侵入後、ジャンプスケア判定が始まるまでの待機時間(秒)")]
    [Range(1f, 60f)]
    [SerializeField] private float lurkDuration = 15f;

    public float MoveSpeed => moveSpeed;
    public float ArriveDistance => arriveDistance;
    public float AppearDuration => appearDuration;
    public float EnterDuration => enterDuration;
    public float ExitVisibleDuration => exitVisibleDuration;
    public float LurkDuration => lurkDuration;
}