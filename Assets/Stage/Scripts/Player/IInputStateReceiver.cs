// 入力状態を外部から変更できることを示すインターフェース
// イベント演出コンポーネントはこのインターフェース経由でPlayerControllerに状態を渡す
public interface IInputStateReceiver
{
    void SetInputState(InputState state);
}