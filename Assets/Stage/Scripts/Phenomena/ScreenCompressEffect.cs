using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// 四方からの圧縮演出(1C-3)
// 発動時にプレイヤー位置へルートを移動し、四方の壁が中央へ向かって締め付ける
// 壁はワールド空間に固定され、プレイヤーの移動に追従しない
// タイムアウトかプレイヤー接触で死亡、アイテム設置成功でStop()を呼ぶと停止・復旧する
public class ScreenCompressEffect : MonoBehaviour
{
    [System.Serializable]
    public struct CompressPanel
    {
        [Tooltip("動かすパネルのTransform")]
        public Transform panel;

        [Tooltip("開いた状態のローカル座標")]
        public Vector2 openPosition;

        [Tooltip("閉じきった状態のローカル座標")]
        public Vector2 closedPosition;
    }

    [Header("参照")]
    [Tooltip("発動時の中心位置を取得するためのプレイヤー参照")]
    [SerializeField] private PlayerReference playerReference;

    [Header("圧縮パネル")]
    [Tooltip("上下左右など、中央へ寄せるパネル群")]
    [SerializeField] private CompressPanel[] panels;

    [Header("時間設定")]
    [Tooltip("完全に閉じるまでの時間(秒)")]
    [Range(1f, 20f)]
    [SerializeField] private float compressDuration = 10f;

    [Tooltip("復旧にかける時間(秒)")]
    [Range(0.1f, 5f)]
    [SerializeField] private float recoverDuration = 0.8f;

    [Header("イベント")]
    [Tooltip("タイムアウトまたは接触で死亡したとき発火")]
    [SerializeField] private UnityEvent onPlayerKilled;

    [Tooltip("停止・復旧が完了したとき発火")]
    [SerializeField] private UnityEvent onStopped;

    private Coroutine compressCoroutine;
    private bool isActive;

    private void Awake()
    {
        // 初期状態では子パネルを含めて非表示にする
        SetPanelsActive(false);
        ResetPanels();
    }

    // 圧縮を開始する。発動時点のプレイヤー位置を中心に展開する
    public void Play()
    {
        if (!isActiveAndEnabled) return;
        if (compressCoroutine != null) StopCoroutine(compressCoroutine);

        // 発動時点のプレイヤー位置へルートを移動(スナップショット)
        if (playerReference != null && playerReference.IsRegistered)
            transform.position = playerReference.PlayerTransform.position;

        ResetPanels();
        SetPanelsActive(true);

        isActive = true;
        compressCoroutine = StartCoroutine(CompressRoutine());
    }

    // 圧縮を停止し復旧する(アイテム設置成功時)
    public void Stop()
    {
        if (!isActive) return;
        isActive = false;
        if (compressCoroutine != null) StopCoroutine(compressCoroutine);
        compressCoroutine = StartCoroutine(RecoverRoutine());
    }

    // プレイヤーに接触したことを外部(パネルのトリガー)から通知する
    public void NotifyPlayerHit()
    {
        if (!isActive) return;
        isActive = false;
        if (compressCoroutine != null) StopCoroutine(compressCoroutine);
        compressCoroutine = null;
        onPlayerKilled.Invoke();
    }

    private IEnumerator CompressRoutine()
    {
        float elapsed = 0f;
        while (elapsed < compressDuration)
        {
            elapsed += Time.deltaTime;
            ApplyPanelPositions(Mathf.Clamp01(elapsed / compressDuration));
            yield return null;
        }

        ApplyPanelPositions(1f);
        isActive = false;
        compressCoroutine = null;
        // タイムアウトで死亡
        onPlayerKilled.Invoke();
    }

    private IEnumerator RecoverRoutine()
    {
        Vector3[] startPositions = new Vector3[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].panel != null)
                startPositions[i] = panels[i].panel.localPosition;
        }

        float elapsed = 0f;
        while (elapsed < recoverDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / recoverDuration);
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].panel == null) continue;
                panels[i].panel.localPosition =
                    Vector3.Lerp(startPositions[i], panels[i].openPosition, t);
            }
            yield return null;
        }

        ResetPanels();
        SetPanelsActive(false);
        compressCoroutine = null;
        onStopped.Invoke();
    }

    private void ApplyPanelPositions(float t)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].panel == null) continue;
            panels[i].panel.localPosition =
                Vector2.Lerp(panels[i].openPosition, panels[i].closedPosition, t);
        }
    }

    private void ResetPanels()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].panel == null) continue;
            panels[i].panel.localPosition = panels[i].openPosition;
        }
    }

    private void SetPanelsActive(bool value)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].panel == null) continue;
            panels[i].panel.gameObject.SetActive(value);
        }
    }
}