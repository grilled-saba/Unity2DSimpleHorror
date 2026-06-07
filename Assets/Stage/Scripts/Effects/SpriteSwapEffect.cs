using UnityEngine;

// 通常スプライトと異常スプライトを切り替える
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapEffect : MonoBehaviour
{
    [Header("スプライト")]
    [Tooltip("通常状態のスプライト")]
    [SerializeField] private Sprite normalSprite;

    [Tooltip("異常状態のスプライト")]
    [SerializeField] private Sprite abnormalSprite;

    [Header("動作設定")]
    [Tooltip("異常化したあと通常に戻せるか")]
    [SerializeField] private bool reversible = true;

    private SpriteRenderer spriteRenderer;
    private bool isAbnormal;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
    }

    // 異常状態へ切り替える
    public void SwitchToAbnormal()
    {
        if (abnormalSprite == null) return;
        spriteRenderer.sprite = abnormalSprite;
        isAbnormal = true;
    }

    // 通常状態へ戻す
    public void SwitchToNormal()
    {
        if (!reversible || normalSprite == null) return;
        spriteRenderer.sprite = normalSprite;
        isAbnormal = false;
    }

    // 状態をトグルする
    public void Toggle()
    {
        if (isAbnormal) SwitchToNormal();
        else SwitchToAbnormal();
    }
}