using UnityEngine;

// プレイヤーに関する数値データ
[CreateAssetMenu(fileName = "PlayerData", menuName = "9T/Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("移動設定")]
    [Tooltip("左右の移動速度")]
    [Range(1f, 20f)]
    [SerializeField] private float moveSpeed = 5f;

    [Header("アイテム操作設定")]
    [Tooltip("拾える範囲の半径")]
    [Range(0.5f, 5f)]
    [SerializeField] private float pickUpRadius = 1.5f;

    public float MoveSpeed => moveSpeed;
    public float PickUpRadius => pickUpRadius;
}