using UnityEngine;

// 現象演出のテスト用入力スクリプト。本番では削除する
public class PhenomenaTestInput : MonoBehaviour
{
    [Header("テスト対象")]
    [SerializeField] private MapFlipEffect mapFlipEffect;

    [Header("浮遊演出テスト")]
    [SerializeField] private FloatingObject furnitureSmall;
    [SerializeField] private FloatingObject furnitureMid;
    [SerializeField] private HeavyFloatingObject furnitureLarge;

    private void Update()
    {
        // Fキーでマップ反転演出を起動
        if (Input.GetKeyDown(KeyCode.F))
        {
            mapFlipEffect.TriggerFlip();
        }

        // G/H/Jキーで各家具の浮遊演出を起動する
        if (Input.GetKeyDown(KeyCode.G)) furnitureSmall?.TriggerFloat();
        if (Input.GetKeyDown(KeyCode.H)) furnitureMid?.TriggerFloat();
        if (Input.GetKeyDown(KeyCode.J)) furnitureLarge?.TriggerFloat();

    }
}