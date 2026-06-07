using UnityEngine;

// 実行時にプレイヤーのTransformを共有するためのScriptableObject
// プレイヤー側が起動時に自身を登録し、プレハブ化された他コンポーネントが参照する
// (Find禁止のルールを守りつつ、プレハブからシーン上のプレイヤーを参照するための仕組み)
[CreateAssetMenu(fileName = "PlayerReference", menuName = "9T/Core/PlayerReference")]
public class PlayerReference : ScriptableObject
{
    private Transform playerTransform;

    public Transform PlayerTransform => playerTransform;
    public bool IsRegistered => playerTransform != null;

    // プレイヤー側から自身を登録する(PlayerControllerのAwakeで呼ぶ想定)
    public void Register(Transform t)
    {
        playerTransform = t;
    }

    // 登録解除(シーン遷移・破棄時に呼ぶ)
    public void Unregister()
    {
        playerTransform = null;
    }
}