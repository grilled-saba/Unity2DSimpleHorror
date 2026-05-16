using UnityEngine;

// 特定のアイテムを持ったときに発動するトリガー
// PlayerControllerのOnItemPickedUp/OnItemDroppedイベントを購読して動作する
public class ItemTrigger : BaseTrigger
{
    [Header("検出設定")]
    [Tooltip("トリガーの対象となるアイテム")]
    [SerializeField] private PickableObject targetItem;

    [Tooltip("プレイヤーのコントローラー")]
    [SerializeField] private PlayerController playerController;

    private void OnEnable()
    {
        playerController.OnItemPickedUp += HandleItemPickedUp;
        playerController.OnItemDropped += HandleItemDropped;
    }

    private void OnDisable()
    {
        playerController.OnItemPickedUp -= HandleItemPickedUp;
        playerController.OnItemDropped -= HandleItemDropped;
    }

    // 拾ったアイテムが対象と一致すれば有効化する
    private void HandleItemPickedUp(PickableObject item)
    {
        if (item != targetItem) return;
        Activate();
    }

    // アイテムを置いたら無効化する
    private void HandleItemDropped()
    {
        Deactivate();
    }
}