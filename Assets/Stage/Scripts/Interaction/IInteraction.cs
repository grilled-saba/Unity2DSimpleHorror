// ボタン1つに対応する個別の相互作用
public interface IInteraction
{
    // 表示するアイコンなどを紐づけるための種別
    InteractionType Type { get; }

    // 相互作用を実行する
    void Execute(IItemHolder holder);
}