using UnityEngine;

// 現象演出のテスト用入力スクリプト。本番では削除する
public class PhenomenaTestInput : MonoBehaviour
{
    [Header("マップ反転テスト")]
    [SerializeField] private MapFlipEffect mapFlipEffect;

    [Header("浮遊演出テスト")]
    [SerializeField] private FloatingObject furnitureSmall;
    [SerializeField] private FloatingObject furnitureMid;
    [SerializeField] private HeavyFloatingObject furnitureLarge;

    [Header("入力状態テスト")]
    [SerializeField] private PlayerController playerController;

    [Header("トリガーテスト")]
    [SerializeField] private StepTrigger stepTrigger;
    [SerializeField] private RangeTrigger rangeTrigger;
    [SerializeField] private ItemTrigger itemTrigger;

    [Header("天井家具テスト")]
    [SerializeField] private CeilingObject ceilingObject;

    private void OnEnable()
    {
        if (stepTrigger != null)
        {
            stepTrigger.OnActivated += OnStepActivated;
            stepTrigger.OnDeactivated += OnStepDeactivated;
        }

        if (rangeTrigger != null)
        {
            rangeTrigger.OnActivated += OnRangeActivated;
            rangeTrigger.OnDeactivated += OnRangeDeactivated;
        }

        if (itemTrigger != null)
        {
            itemTrigger.OnActivated += OnItemActivated;
            itemTrigger.OnDeactivated += OnItemDeactivated;
        }
    }

    private void OnDisable()
    {
        if (stepTrigger != null)
        {
            stepTrigger.OnActivated -= OnStepActivated;
            stepTrigger.OnDeactivated -= OnStepDeactivated;
        }

        if (rangeTrigger != null)
        {
            rangeTrigger.OnActivated -= OnRangeActivated;
            rangeTrigger.OnDeactivated -= OnRangeDeactivated;
        }

        if (itemTrigger != null)
        {
            itemTrigger.OnActivated -= OnItemActivated;
            itemTrigger.OnDeactivated -= OnItemDeactivated;
        }
    }

    private void Update()
    {
        // Fキーでマップ上下反転を切り替える
        if (Input.GetKeyDown(KeyCode.F))
            mapFlipEffect?.TriggerFlip();

        // Rキーでマップのランダム360度回転を起動する
        if (Input.GetKeyDown(KeyCode.R))
            mapFlipEffect?.TriggerRandomRotation();

        // G/H/Jキーで各家具の浮遊演出を起動する
        if (Input.GetKeyDown(KeyCode.G)) furnitureSmall?.TriggerFloat();
        if (Input.GetKeyDown(KeyCode.H)) furnitureMid?.TriggerFloat();
        if (Input.GetKeyDown(KeyCode.J)) furnitureLarge?.TriggerFloat();

        // Lキーで入力をロックし、プレイヤーの操作を完全に無効化する
        if (Input.GetKeyDown(KeyCode.L))
            playerController?.SetInputState(InputState.Locked);

        // Nキーで入力状態を通常に戻す
        if (Input.GetKeyDown(KeyCode.N))
            playerController?.SetInputState(InputState.Normal);

        // Iキーで左右入力を反転させる
        if (Input.GetKeyDown(KeyCode.I))
            playerController?.SetInputState(InputState.Inverted);

        // Cキーで天井家具の落下を起動する
        if (Input.GetKeyDown(KeyCode.C))
            ceilingObject?.TriggerDrop();
    }

    private void OnStepActivated() => Debug.Log("StepTrigger: 有効化");
    private void OnStepDeactivated() => Debug.Log("StepTrigger: 無効化");

    private void OnRangeActivated() => Debug.Log("RangeTrigger: 有効化");
    private void OnRangeDeactivated() => Debug.Log("RangeTrigger: 無効化");

    private void OnItemActivated() => Debug.Log("ItemTrigger: 有効化");
    private void OnItemDeactivated() => Debug.Log("ItemTrigger: 無効化");
}