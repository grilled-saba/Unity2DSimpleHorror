using UnityEngine;
using UnityEngine.Events;

// 指定した全トリガーが有効になったときに発動するトリガー（AND条件）
public class CompositeTrigger : MonoBehaviour
{
    [Header("監視対象のトリガー（すべて有効で発動）")]
    [SerializeField] private BaseTrigger[] triggers;

    [Header("イベント")]
    [Tooltip("全トリガーが有効になったときに呼ばれるイベント")]
    [SerializeField] private UnityEvent onAllActivated;

    [Tooltip("いずれかのトリガーが無効になったときに呼ばれるイベント")]
    [SerializeField] private UnityEvent onAnyDeactivated;

    private bool wasAllActivated = false;

    private void OnEnable()
    {
        foreach (BaseTrigger trigger in triggers)
        {
            trigger.OnActivated += CheckCondition;
            trigger.OnDeactivated += CheckCondition;
        }
    }

    private void OnDisable()
    {
        foreach (BaseTrigger trigger in triggers)
        {
            trigger.OnActivated -= CheckCondition;
            trigger.OnDeactivated -= CheckCondition;
        }
    }

    // いずれかのトリガー状態が変化したときに全体の条件を再評価する
    private void CheckCondition()
    {
        bool allActivated = AreAllActivated();

        if (allActivated && !wasAllActivated)
        {
            wasAllActivated = true;
            onAllActivated?.Invoke();
        }
        else if (!allActivated && wasAllActivated)
        {
            wasAllActivated = false;
            onAnyDeactivated?.Invoke();
        }
    }

    private bool AreAllActivated()
    {
        foreach (BaseTrigger trigger in triggers)
        {
            if (!trigger.IsActivated) return false;
        }
        return true;
    }
}