using UnityEngine;
using UnityEngine.Events;

// アイテムの設置先。正しいアイテムが設置されたとき発火する
// プレイヤーの設置処理(PlayerController側)から TryPlace を呼び出して連携する
public class PlaceTarget : MonoBehaviour
{
    [Header("設置条件")]
    [Tooltip("この設置先が受け入れるアイテムの識別子")]
    [SerializeField] private string acceptedItemId;

    [Header("イベント")]
    [Tooltip("正しいアイテムが設置されたとき発火")]
    [SerializeField] private UnityEvent onCorrectItemPlaced;

    [Tooltip("誤ったアイテムが設置されたとき発火")]
    [SerializeField] private UnityEvent onWrongItemPlaced;

    private bool isFilled;

    // 設置を試みる。識別子が一致すれば成功しtrueを返す
    public bool TryPlace(string itemId)
    {
        if (isFilled) return false;

        if (itemId == acceptedItemId)
        {
            isFilled = true;
            onCorrectItemPlaced.Invoke();
            return true;
        }

        onWrongItemPlaced.Invoke();
        return false;
    }

    public bool IsFilled => isFilled;
    public string AcceptedItemId => acceptedItemId;
}