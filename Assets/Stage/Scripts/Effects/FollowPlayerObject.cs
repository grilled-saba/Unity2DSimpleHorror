using UnityEngine;

// プレイヤーをゆっくり追従する不気味なオブジェクト
// プレハブ化のため、シーン参照ではなくPlayerReference(SO)経由でプレイヤーを取得する
public class FollowPlayerObject : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("プレイヤー参照を保持するScriptableObject")]
    [SerializeField] private PlayerReference playerReference;

    [Header("追従設定")]
    [Tooltip("追従速度")]
    [Range(0.1f, 10f)]
    [SerializeField] private float followSpeed = 1.5f;

    [Tooltip("この距離以下には近づかない")]
    [Range(0f, 10f)]
    [SerializeField] private float stopDistance = 2f;

    [Tooltip("起動時から追従を開始するか")]
    [SerializeField] private bool followOnStart = false;

    private bool isFollowing;

    private void Start()
    {
        isFollowing = followOnStart;
    }

    // 追従を開始する
    public void StartFollow()
    {
        isFollowing = true;
    }

    // 追従を停止する
    public void StopFollow()
    {
        isFollowing = false;
    }

    private void Update()
    {
        if (!isFollowing || playerReference == null) return;

        Transform target = playerReference.PlayerTransform;
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= stopDistance) return;

        transform.position = Vector2.MoveTowards(
            transform.position, target.position, followSpeed * Time.deltaTime);
    }
}