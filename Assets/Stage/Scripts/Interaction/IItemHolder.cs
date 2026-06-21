// アイテムを持ち運び・設置するプレイヤー側の機能
public interface IItemHolder
{
    // 現在アイテムを持っているか
    bool IsHolding { get; }

    // 持っているアイテムの識別子。持っていないときはnull
    string HeldItemId { get; }

    // アイテムを持ち上げる
    void Hold(PickableObject pickable);

    // 持っているアイテムを設置先へ置く
    void PlaceHeldItemOn(PlaceTarget target);
}