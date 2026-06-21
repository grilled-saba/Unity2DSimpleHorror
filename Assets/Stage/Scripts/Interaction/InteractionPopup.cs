using System.Collections.Generic;
using UnityEngine;

// 1つのオブジェクトに付随する相互作用ボタンのまとまり。
// 横並びでボタンを生成し、順番に出現させる。
public class InteractionPopup : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("生成するボタンのプレハブ")]
    [SerializeField] private InteractionButtonView buttonPrefab;

    [Header("出現")]
    [Tooltip("ボタンを1つずつ出現させる間隔（秒）")]
    [SerializeField] private float staggerDelay = 0.06f;

    // 相互作用のリストからボタンを生成し、順番に出現させる
    public void Show(IReadOnlyList<IInteraction> interactions, IItemHolder holder)
    {
        for (int i = 0; i < interactions.Count; i++)
        {
            InteractionButtonView button = Instantiate(buttonPrefab);
            button.transform.SetParent(transform, false);
            button.Bind(interactions[i], holder);
            button.Reveal(staggerDelay * i);
        }
    }
}