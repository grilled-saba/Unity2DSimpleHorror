using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;

// 相互作用可能なオブジェクトが実装するインターフェース
public interface IInteractable
{
    // プレイヤーの所持状況をもとに、現在実行可能な相互作用のみを返す
    IReadOnlyList<IInteraction> GetAvailableInteractions(IItemHolder holder);
}