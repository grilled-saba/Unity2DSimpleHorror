using System.Collections;
using UnityEngine;

// 浮遊家具の共通処理を定義する基底クラス。
// FloatingObject・HeavyFloatingObjectはこのクラスを継承する。
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class BaseFloatingObject : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private FloatingObjectData floatingData;

    [Header("エフェクト")]
    [SerializeField] private FloatingObjectEffect effect;

    [Tooltip("着地時に発動するAOEコンポーネント。未設定の場合はAOEなし")]
    [SerializeField] private AoeEffect aoeEffect;

    private Rigidbody2D rb;
    private bool isActive = false;
    private bool hasTriggeredAoe = false;
    private bool isHoldingFloat = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        OnAwake();
    }

    // サブクラスで追加の初期化が必要な場合にオーバーライドする
    protected virtual void OnAwake() { }

    // 浮遊演出を外部から起動する
    // 通常は上昇→維持→自動落下。データのHoldIndefinitelyがONなら降りずに維持し続ける
    public void TriggerFloat()
    {
        if (isActive) return;
        StartCoroutine(FloatSequence());
    }

    // 上昇して浮遊状態を維持する。Restoreが呼ばれるまで降りない
    // (データを共有しつつ、この呼び出しだけ維持させたい場合に使う)
    public void TriggerFloatHold()
    {
        if (isActive) return;
        StartCoroutine(FloatHoldSequence());
    }

    // 維持中の浮遊を終了し、落下させて通常状態へ戻す
    public void Restore()
    {
        if (!isHoldingFloat) return;
        isHoldingFloat = false;

        effect?.StopFloatEffect();
        rb.simulated = true;
        StartCoroutine(RestoreCooldown());
    }

    // 浮遊から落下までの一連の流れ
    private IEnumerator FloatSequence()
    {
        isActive = true;
        hasTriggeredAoe = false;

        rb.simulated = false;
        effect?.PlayFloatEffect();

        yield return RiseRoutine();

        // 継続浮遊が指定されている場合は降りずに維持する(復旧はRestoreで行う)
        if (floatingData.HoldIndefinitely)
        {
            isHoldingFloat = true;
            yield break;
        }

        // 浮遊状態を一定時間維持する
        yield return new WaitForSeconds(floatingData.FloatHoldDuration);

        effect?.StopFloatEffect();

        // 物理を再び有効にして落下させる
        rb.simulated = true;

        // 3秒以内に衝突がなければ非アクティブに戻す
        yield return new WaitForSeconds(3f);
        isActive = false;
    }

    // 上昇して維持し続ける流れ(落下はRestoreで行う)
    private IEnumerator FloatHoldSequence()
    {
        isActive = true;
        hasTriggeredAoe = false;

        rb.simulated = false;
        effect?.PlayFloatEffect();

        yield return RiseRoutine();

        isHoldingFloat = true;
    }

    // 指定時間をかけて上昇する共通処理
    private IEnumerator RiseRoutine()
    {
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + Vector2.up * floatingData.FloatHeight;
        float elapsed = 0f;

        while (elapsed < floatingData.FloatDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / floatingData.FloatDuration);
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    // 復旧後の衝突判定を少し待ってから非アクティブへ戻す
    private IEnumerator RestoreCooldown()
    {
        yield return new WaitForSeconds(3f);
        isActive = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;
        if (hasTriggeredAoe) return;
        if (!ShouldTriggerAoe(collision)) return;

        hasTriggeredAoe = true;
        isActive = false;

        effect?.PlayLandEffect();
        aoeEffect?.Trigger();
    }

    // AOEを発動すべき衝突かを判定する。
    // 基底実装は常にtrueを返す。サブクラスで条件を上書きできる
    protected virtual bool ShouldTriggerAoe(Collision2D collision)
    {
        return true;
    }
}