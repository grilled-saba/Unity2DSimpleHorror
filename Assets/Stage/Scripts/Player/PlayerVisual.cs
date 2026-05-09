using System.Collections;
using UnityEngine;

// プレイヤーの色演出を管理する
[RequireComponent(typeof(PlayerHealth))]
public class PlayerVisual : MonoBehaviour
{
    [Header("ビジュアル設定")]
    [SerializeField] private PlayerHealthData healthData;

    [Tooltip("危険度の色を重ねるオーバーレイスプライト")]
    [SerializeField] private SpriteRenderer visualDanger;

    private PlayerHealth playerHealth;
    private Coroutine flashCoroutine;
    private Coroutine blendCoroutine;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();

        // オーバーレイは初期状態で透明にする
        if (visualDanger != null)
        {
            visualDanger.color = Color.clear;
        }
    }

    private void OnEnable()
    {
        playerHealth.OnHpChanged += HandleHpChanged;
        playerHealth.OnDamaged += HandleDamaged;
    }

    private void OnDisable()
    {
        playerHealth.OnHpChanged -= HandleHpChanged;
        playerHealth.OnDamaged -= HandleDamaged;
    }

    // HP変化時に危険度の色へブレンドする
    private void HandleHpChanged(int currentHp, int maxHp)
    {
        float ratio = (float)currentHp / maxHp;

        if (ratio > 0.66f)
        {
            StartBlend(visualDanger.color, Color.clear);
            return;
        }

        Color dangerColor = healthData.GetDangerColor(currentHp);

        Color targetColor = new Color(
            dangerColor.r,
            dangerColor.g,
            dangerColor.b,
            0.5f
        );

        StartBlend(visualDanger.color, targetColor);
    }

    // ダメージ時の点滅演出を開始する
    private void HandleDamaged()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private void StartBlend(Color from, Color to)
    {
        if (blendCoroutine != null)
        {
            StopCoroutine(blendCoroutine);
        }

        blendCoroutine = StartCoroutine(BlendCoroutine(from, to));
    }

    // 点滅演出後に危険度の色へ徐々にブレンドする
    private IEnumerator FlashCoroutine()
    {
        float elapsed = 0f;
        Color flashColor = new Color(
            healthData.DamageFlashColor.r,
            healthData.DamageFlashColor.g,
            healthData.DamageFlashColor.b,
            0.8f
        );

        while (elapsed < healthData.FlashDuration)
        {
            visualDanger.color = flashColor;
            yield return new WaitForSeconds(healthData.FlashInterval);

            visualDanger.color = Color.clear;
            yield return new WaitForSeconds(healthData.FlashInterval);

            elapsed += healthData.FlashInterval * 2f;
        }

        // 点滅終了後、現在の危険度色へブレンド
        float ratio = (float)playerHealth.CurrentHp / playerHealth.MaxHp;
        Color dangerColor = healthData.GetDangerColor(playerHealth.CurrentHp);
        Color targetColor = ratio > 0.66f
            ? Color.clear
            : new Color(dangerColor.r, dangerColor.g, dangerColor.b, 0.5f);

        StartBlend(visualDanger.color, targetColor);
    }

    // 指定時間をかけて色をブレンドする
    private IEnumerator BlendCoroutine(Color from, Color to)
    {
        float elapsed = 0f;

        while (elapsed < healthData.BlendDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / healthData.BlendDuration);
            visualDanger.color = Color.Lerp(from, to, t);
            yield return null;
        }

        visualDanger.color = to;
    }
}