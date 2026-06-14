using UnityEngine;

// プレイヤーの向きを移動方向に合わせて切り替えるコンポーネント。
// スプライトはflipXで反転させ、向き依存のオブジェクト（HoldPointや拾い判定範囲）は
// ローカルX座標の符号のみ反転させる。
// ライティング(Shadow Caster 2D / Light2D)への影響を避けるため、スケール反転は行わない。
public class PlayerFacing : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("プレイヤー本体のSpriteRenderer。flipXで左右を反転させる")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("向きに応じてローカルX座標の符号を反転させる対象（HoldPoint、拾い判定範囲など）")]
    [SerializeField] private Transform[] directionAnchors;

    [Tooltip("向きの変化を通知するコンポーネント(IFacingNotifierを実装したもの)")]
    [SerializeField] private MonoBehaviour facingNotifierSource;

    [Header("設定")]
    [Tooltip("元のスプライト素材が右を向いているか")]
    [SerializeField] private bool spriteFacesRight = true;

    // 通知元のインターフェース参照
    private IFacingNotifier facingNotifier;
    // 現在右を向いているか
    private bool facingRight = true;
    // 各対象の初期ローカル座標。X符号反転の基準として保持する
    private Vector3[] anchorBasePositions;

    private void Awake()
    {
        facingNotifier = facingNotifierSource as IFacingNotifier;

        if (directionAnchors != null)
        {
            anchorBasePositions = new Vector3[directionAnchors.Length];
            for (int i = 0; i < directionAnchors.Length; i++)
            {
                if (directionAnchors[i] != null)
                    anchorBasePositions[i] = directionAnchors[i].localPosition;
            }
        }
    }

    private void OnEnable()
    {
        if (facingNotifier != null)
            facingNotifier.OnFacingChanged += HandleFacingChanged;
    }

    private void OnDisable()
    {
        if (facingNotifier != null)
            facingNotifier.OnFacingChanged -= HandleFacingChanged;
    }

    // 向き変化通知を受け取り、現在の向きを更新する
    private void HandleFacingChanged(bool isFacingRight)
    {
        facingRight = isFacingRight;
    }

    // Animatorがスプライトを更新した後に反転を適用するため、LateUpdateで処理する
    private void LateUpdate()
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = facingRight != spriteFacesRight;

        if (directionAnchors == null) return;

        float sign = facingRight ? 1f : -1f;
        for (int i = 0; i < directionAnchors.Length; i++)
        {
            Transform anchor = directionAnchors[i];
            if (anchor == null) continue;
            Vector3 basePos = anchorBasePositions[i];
            anchor.localPosition = new Vector3(Mathf.Abs(basePos.x) * sign, basePos.y, basePos.z);
        }
    }
}