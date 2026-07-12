using System.Collections.Generic;
using UnityEngine;

// 範囲内の相互作用可能オブジェクトに対し、相互作用ボタンのポップアップを表示・管理する。
public class InteractionPopupController : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("前方の相互作用判定範囲")]
    [SerializeField] private InteractionDetector detector;
    [Tooltip("アイテムの所持主体（PlayerController）")]
    [SerializeField] private PlayerController player;
    [Tooltip("ポップアップを配置するWorld Spaceキャンバス")]
    [SerializeField] private Transform popupCanvas;
    [Tooltip("ポップアップのプレハブ")]
    [SerializeField] private InteractionPopup popupPrefab;

    [Header("配置")]
    [Tooltip("オブジェクトからボタン表示位置までのオフセット（右側）")]
    [SerializeField] private Vector2 offset = new Vector2(1f, 0f);

    // 現在表示中のポップアップ
    private readonly Dictionary<IInteractable, InteractionPopup> popups =
        new Dictionary<IInteractable, InteractionPopup>();
    // 再利用する作業用リスト
    private readonly List<IInteractable> activeInteractables = new List<IInteractable>();
    private readonly List<IInteractable> toRemove = new List<IInteractable>();

    private void Awake()
    {
        // マウスカーソルを表示する
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnEnable()
    {
        detector.OnInteractablesChanged += Refresh;
        player.OnItemPickedUp += OnHeldChanged;
        player.OnItemDropped += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        detector.OnInteractablesChanged -= Refresh;
        player.OnItemPickedUp -= OnHeldChanged;
        player.OnItemDropped -= Refresh;
    }

    // OnItemPickedUpはPickableObjectを引数に取るため、引数を捨ててRefreshする
    private void OnHeldChanged(PickableObject pickable)
    {
        Refresh();
    }

    // 表示中のポップアップを対象オブジェクトの現在位置に追従させる。
    // 位置のみ複製するため、対象が回転してもポップアップは回転しない
    private void LateUpdate()
    {
        foreach (KeyValuePair<IInteractable, InteractionPopup> pair in popups)
        {
            if (pair.Key as UnityEngine.Object == null || pair.Value == null) continue;
            pair.Value.transform.position = GetPopupPosition(pair.Key);
        }
    }

    // 対象オブジェクトの位置からポップアップの表示位置を求める
    private Vector3 GetPopupPosition(IInteractable interactable)
    {
        Transform anchor = ((MonoBehaviour)interactable).transform;
        return anchor.position + (Vector3)offset;
    }

    // 表示すべきポップアップを再計算し、過不足を反映する
    private void Refresh()
    {
        // 相互作用があるオブジェクトを集める
        activeInteractables.Clear();
        foreach (IInteractable interactable in detector.Interactables)
        {
            // 破棄済みは無視する
            if (interactable as UnityEngine.Object == null) continue;
            if (interactable.GetAvailableInteractions(player).Count > 0)
            {
                activeInteractables.Add(interactable);
            }
        }

        // 不要になったポップアップを削除する
        toRemove.Clear();
        foreach (KeyValuePair<IInteractable, InteractionPopup> pair in popups)
        {
            if (pair.Key as UnityEngine.Object == null || !activeInteractables.Contains(pair.Key))
            {
                if (pair.Value != null) Destroy(pair.Value.gameObject);
                toRemove.Add(pair.Key);
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            popups.Remove(toRemove[i]);
        }

        // 新しく必要になったポップアップを生成する
        foreach (IInteractable interactable in activeInteractables)
        {
            if (popups.ContainsKey(interactable)) continue;

            InteractionPopup popup = Instantiate(popupPrefab);
            popup.transform.SetParent(popupCanvas, false);
            popup.transform.position = GetPopupPosition(interactable);

            popup.Show(interactable.GetAvailableInteractions(player), player);
            popups.Add(interactable, popup);
        }
    }
}