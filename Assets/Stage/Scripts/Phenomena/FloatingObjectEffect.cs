using UnityEngine;

// 浮遊演出のエフェクト再生を管理する
// パーティクル・スプライトどちらにも対応できる構造にしている
public class FloatingObjectEffect : MonoBehaviour
{
    [Header("浮遊中のエフェクト")]
    [Tooltip("浮遊中に再生するパーティクル（未設定でもOK）")]
    [SerializeField] private ParticleSystem floatParticle;

    [Tooltip("浮遊中に表示するスプライト（未設定でもOK）")]
    [SerializeField] private SpriteRenderer floatSprite;

    [Header("着地時のエフェクト")]
    [Tooltip("着地時に再生するパーティクル（未設定でもOK）")]
    [SerializeField] private ParticleSystem landParticle;

    [Tooltip("着地時に表示するスプライト（未設定でもOK）")]
    [SerializeField] private SpriteRenderer landSprite;

    [Tooltip("着地スプライトの表示時間（秒）")]
    [Range(0.1f, 2f)]
    [SerializeField] private float landSpriteDisplayDuration = 0.3f;

    // 浮遊エフェクトを開始する
    public void PlayFloatEffect()
    {
        if (floatParticle != null) floatParticle.Play();
        if (floatSprite != null) floatSprite.enabled = true;
    }

    // 浮遊エフェクトを停止する
    public void StopFloatEffect()
    {
        if (floatParticle != null) floatParticle.Stop();
        if (floatSprite != null) floatSprite.enabled = false;
    }

    // 着地エフェクトを再生する
    public void PlayLandEffect()
    {
        if (landParticle != null) landParticle.Play();
        if (landSprite != null) StartCoroutine(ShowLandSprite());
    }

    // 着地スプライトを一定時間だけ表示する
    private System.Collections.IEnumerator ShowLandSprite()
    {
        landSprite.enabled = true;
        yield return new WaitForSeconds(landSpriteDisplayDuration);
        landSprite.enabled = false;
    }
}