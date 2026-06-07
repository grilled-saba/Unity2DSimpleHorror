using UnityEngine;

// シーンビューでCollider2Dの範囲を可視化するデバッグ補助コンポーネント
// トリガーや設置先などの当たり判定をレベルデザイン時に見やすくする
// ゲーム動作には影響せず、ギズモはエディタ上でのみ描画される
[RequireComponent(typeof(Collider2D))]
public class GizmoVisualizer : MonoBehaviour
{
    [Header("表示設定")]
    [Tooltip("ギズモの色")]
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 1f, 0.25f);

    [Tooltip("選択時のみ表示するか")]
    [SerializeField] private bool onlyWhenSelected = false;

    [Tooltip("ラベルとして表示する文字列(任意)")]
    [SerializeField] private string label = "";

    private Collider2D col;

    private void OnDrawGizmos()
    {
        if (onlyWhenSelected) return;
        DrawGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (!onlyWhenSelected) return;
        DrawGizmo();
    }

    private void DrawGizmo()
    {
        if (col == null) col = GetComponent<Collider2D>();
        if (col == null) return;

        Bounds b = col.bounds;

        // 半透明の塗りつぶし
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(b.center, b.size);

        // 不透明の枠線
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawWireCube(b.center, b.size);

#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(label))
            UnityEditor.Handles.Label(b.center, label);
#endif
    }
}