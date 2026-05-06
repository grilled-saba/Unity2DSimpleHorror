using UnityEngine;

// 現象演出のテスト用入力スクリプト。本番では削除する
public class PhenomenaTestInput : MonoBehaviour
{
    [Header("テスト対象")]
    [SerializeField] private MapFlipEffect mapFlipEffect;

    private void Update()
    {
        // Fキーでマップ反転演出を起動
        if (Input.GetKeyDown(KeyCode.F))
        {
            mapFlipEffect.TriggerFlip();
        }
    }
}