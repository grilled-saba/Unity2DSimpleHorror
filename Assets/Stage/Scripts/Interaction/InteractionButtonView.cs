using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 1つの相互作用に対応するボタン。バブル状に出現し、出現完了までクリックを受け付けない。
[RequireComponent(typeof(CanvasGroup))]
public class InteractionButtonView : MonoBehaviour
{
    [Header("出現アニメーション")]
    [Tooltip("出現にかける時間（秒）")]
    [SerializeField] private float popDuration = 0.18f;
    [Tooltip("スケール変化カーブ。終端を1より少し上にするとバブルらしく弾む")]
    [SerializeField] private AnimationCurve popCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("参照")]
    [Tooltip("クリックを受け取るButton")]
    [SerializeField] private Button button;
    [Tooltip("種別ごとにスプライトを差し替えるImage（ボタン自身のImageでよい）")]
    [SerializeField] private Image iconImage;
    [Tooltip("種別ごとのアイコン定義")]
    [SerializeField] private InteractionIconSet iconSet;

    private CanvasGroup canvasGroup;
    private IInteraction interaction;
    private IItemHolder holder;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // 表示する相互作用と実行主体を結びつける
    // 表示する相互作用と実行主体を結びつける
    public void Bind(IInteraction interaction, IItemHolder holder)
    {
        this.interaction = interaction;
        this.holder = holder;
        button.onClick.AddListener(OnClicked);
        ApplyIcon(interaction.Type);
    }

    // 種別に応じてアイコンを差し替える
    // 種別に応じて通常時・ホバー時スプライトを差し替える
    private void ApplyIcon(InteractionType type)
    {
        if (iconImage == null || iconSet == null || button == null) return;

        if (iconSet.TryGetSprites(type, out Sprite normal, out Sprite highlighted))
        {
            // 通常時はImageのsprite、ホバー時はButtonのSpriteStateで切り替わる
            iconImage.sprite = normal;

            SpriteState state = button.spriteState;
            state.highlightedSprite = highlighted;
            button.spriteState = state;
        }
        else
        {
            Debug.LogWarning($"[InteractionButtonView] {type}に対応するスプライトがInteractionIconSetに未設定です。");
        }
    }

    // 指定した遅延のあとに出現アニメーションを再生する
    public void Reveal(float delay)
    {
        StartCoroutine(RevealRoutine(delay));
    }

    private IEnumerator RevealRoutine(float delay)
    {
        // 出現前は非表示・クリック不可
        transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (delay > 0f) yield return new WaitForSeconds(delay);

        canvasGroup.alpha = 1f;

        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popDuration);
            float scale = popCurve.Evaluate(t);
            transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;

        // 出現完了後にクリックを受け付ける
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnClicked()
    {
        interaction.Execute(holder);
    }
}