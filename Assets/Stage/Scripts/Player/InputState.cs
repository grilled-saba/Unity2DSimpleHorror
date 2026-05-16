// プレイヤーの入力状態を表す列挙型
public enum InputState
{
    // 通常の入力を受け付ける
    Normal,

    // 入力を完全に遮断する（イベント演出中などに使用）
    Locked,

    // 左右入力を反転させる（マップ反転演出との連動に使用）
    Inverted
}