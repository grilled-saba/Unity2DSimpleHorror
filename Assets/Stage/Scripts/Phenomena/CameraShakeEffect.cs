using UnityEngine;

// カメラを揺らすトラウマベースのスクリーンシェイク
// 画面振動(6 表彰状 / 15 スリッパ / 16 本 / 20 蛇口 / 22 テレビ)で使用する
// 他のカメラ制御(追従など)と競合しないよう、専用の親オブジェクトに付けることを推奨
public class CameraShakeEffect : MonoBehaviour
{
    [Header("シェイク強度")]
    [Tooltip("最大の位置ずれ(ユニット)")]
    [Range(0f, 3f)]
    [SerializeField] private float maxOffset = 0.5f;

    [Tooltip("最大の回転角(度)")]
    [Range(0f, 30f)]
    [SerializeField] private float maxAngle = 5f;

    [Header("減衰")]
    [Tooltip("1秒あたりのトラウマ減衰量。大きいほど早く収まる")]
    [Range(0.1f, 5f)]
    [SerializeField] private float traumaDecay = 1.5f;

    [Tooltip("揺れのノイズの速さ")]
    [Range(1f, 60f)]
    [SerializeField] private float frequency = 25f;

    private float trauma;
    private Vector3 originPosition;
    private Quaternion originRotation;
    private float seed;

    private void Awake()
    {
        originPosition = transform.localPosition;
        originRotation = transform.localRotation;
        seed = Random.value * 100f;
    }

    // トラウマを加える(0〜1で累積)。複数回呼ぶと揺れが強まる
    public void AddTrauma(float amount)
    {
        trauma = Mathf.Clamp01(trauma + amount);
    }

    // 弱いシェイク(アイテム拾得時の1秒振動など)
    public void ShakeSmall()
    {
        AddTrauma(0.25f);
    }

    // 強いシェイク(ジャンプスケア同時発動など)
    public void ShakeLarge()
    {
        AddTrauma(0.7f);
    }

    private void Update()
    {
        if (trauma <= 0f)
        {
            transform.localPosition = originPosition;
            transform.localRotation = originRotation;
            return;
        }

        // trauma^2で揺れの立ち上がりを自然にする
        float shake = trauma * trauma;
        float time = Time.time * frequency;

        // Perlinノイズ(-1〜1)で滑らかにランダムな揺れを作る
        float offsetX = maxOffset * shake * (Mathf.PerlinNoise(seed, time) * 2f - 1f);
        float offsetY = maxOffset * shake * (Mathf.PerlinNoise(seed + 1f, time) * 2f - 1f);
        float angle = maxAngle * shake * (Mathf.PerlinNoise(seed + 2f, time) * 2f - 1f);

        transform.localPosition = originPosition + new Vector3(offsetX, offsetY, 0f);
        transform.localRotation = originRotation * Quaternion.Euler(0f, 0f, angle);

        trauma = Mathf.Clamp01(trauma - traumaDecay * Time.deltaTime);
    }
}