using System;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーの前方に配置する相互作用判定範囲。
// 範囲内のIInteractableを収集し、増減があったとき通知する。
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class InteractionDetector : MonoBehaviour
{
    // 範囲内の相互作用可能オブジェクト
    private readonly List<IInteractable> interactables = new List<IInteractable>();

    // 範囲内のオブジェクトが増減したとき発火
    public event Action OnInteractablesChanged;

    // 現在範囲内の相互作用可能オブジェクト
    public IReadOnlyList<IInteractable> Interactables => interactables;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !interactables.Contains(interactable))
        {
            interactables.Add(interactable);
            OnInteractablesChanged?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactables.Remove(interactable))
        {
            OnInteractablesChanged?.Invoke();
        }
    }
}