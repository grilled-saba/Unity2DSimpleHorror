using UnityEngine;

// プレイヤーオブジェクトに付け、起動時にPlayerReference(SO)へ自身のTransformを登録する
// PlayerControllerを直接編集せずにPlayerReferenceの仕組みを導入するための補助コンポーネント
public class PlayerReferenceRegistrar : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("登録先のPlayerReference")]
    [SerializeField] private PlayerReference playerReference;

    private void Awake()
    {
        if (playerReference != null) playerReference.Register(transform);
    }

    private void OnDestroy()
    {
        if (playerReference != null) playerReference.Unregister();
    }
}