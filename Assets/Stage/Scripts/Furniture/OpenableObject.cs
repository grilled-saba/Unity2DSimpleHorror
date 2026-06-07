using UnityEngine;
using UnityEngine.Events;

// 開閉できる家具(引き出し・扉・戸棚など)
public class OpenableObject : MonoBehaviour
{
    [Header("開閉スプライト")]
    [Tooltip("閉じた状態のスプライト")]
    [SerializeField] private Sprite closedSprite;

    [Tooltip("開いた状態のスプライト")]
    [SerializeField] private Sprite openedSprite;

    [Header("参照")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("動作設定")]
    [Tooltip("一度開けたら閉じられないか")]
    [SerializeField] private bool openOnce = true;

    [Header("イベント")]
    [Tooltip("開いた瞬間に発火")]
    [SerializeField] private UnityEvent onOpened;

    [Tooltip("閉じた瞬間に発火")]
    [SerializeField] private UnityEvent onClosed;

    private bool isOpened;
    private bool hasOpened;

    private void Awake()
    {
        if (spriteRenderer != null && closedSprite != null)
            spriteRenderer.sprite = closedSprite;
    }

    // 外部から開ける
    public void Open()
    {
        if (isOpened) return;
        if (openOnce && hasOpened) return;

        isOpened = true;
        hasOpened = true;
        if (spriteRenderer != null && openedSprite != null)
            spriteRenderer.sprite = openedSprite;
        onOpened.Invoke();
    }

    // 外部から閉じる
    public void Close()
    {
        if (!isOpened || openOnce) return;
        isOpened = false;
        if (spriteRenderer != null && closedSprite != null)
            spriteRenderer.sprite = closedSprite;
        onClosed.Invoke();
    }

    // 開閉トグル
    public void Toggle()
    {
        if (isOpened) Close();
        else Open();
    }

    public bool IsOpened => isOpened;
}