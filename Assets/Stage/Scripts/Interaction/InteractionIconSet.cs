using System;
using UnityEngine;

// 相互作用の種別ごとに、ボタンの通常時・ホバー時スプライトを定義するScriptableObject
[CreateAssetMenu(fileName = "InteractionIconSet", menuName = "9T/Interaction Icon Set")]
public class InteractionIconSet : ScriptableObject
{
    // 種別と、通常時・ホバー時スプライトの対応1組
    [Serializable]
    public struct Entry
    {
        public InteractionType type;
        public Sprite normal;
        public Sprite highlighted;
    }

    [Tooltip("種別ごとのスプライト一覧")]
    [SerializeField] private Entry[] entries;

    // 指定した種別のスプライト組を返す。見つかればtrue
    public bool TryGetSprites(InteractionType type, out Sprite normal, out Sprite highlighted)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].type == type)
            {
                normal = entries[i].normal;
                highlighted = entries[i].highlighted;
                return true;
            }
        }
        normal = null;
        highlighted = null;
        return false;
    }
}