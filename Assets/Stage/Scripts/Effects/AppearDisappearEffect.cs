using System.Collections;
using UnityEngine;

// オブジェクトをフェードで出現・消失させる
[RequireComponent(typeof(SpriteRenderer))]
public class AppearDisappearEffect : MonoBehaviour
{
    [Header("フェード設定")]
    [Tooltip("フェード時間(秒)")]
    [Range(0.05f, 5f)]
    [SerializeField] private float fadeDuration = 0.5f;

    [Tooltip("起動時に非表示状態から始めるか")]
    [SerializeField] private bool startHidden = true;

    private SpriteRenderer spriteRenderer;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (startHidden) SetAlpha(0f);
    }

    // 出現させる
    public void Appear()
    {
        if (!isActiveAndEnabled) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(1f));
    }

    // 消失させる
    public void Disappear()
    {
        if (!isActiveAndEnabled) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(0f));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = spriteRenderer.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));
            yield return null;
        }

        SetAlpha(targetAlpha);
        fadeCoroutine = null;
    }

    private void SetAlpha(float a)
    {
        Color c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;
    }
}