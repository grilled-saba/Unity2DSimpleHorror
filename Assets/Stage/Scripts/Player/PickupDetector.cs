using System.Collections.Generic;
using UnityEngine;

// プレイヤーの前方に配置する拾い判定範囲。
// 範囲内のPickableObjectとPlaceTargetを管理し、クリック地点に基づく問い合わせに応じる。
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PickupDetector : MonoBehaviour
{
    // 範囲内の拾えるオブジェクト
    private readonly List<PickableObject> pickables = new List<PickableObject>();
    // 範囲内の設置先
    private readonly List<PlaceTarget> placeTargets = new List<PlaceTarget>();
    // 自身の範囲コライダー
    private Collider2D zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PickableObject pickable = other.GetComponent<PickableObject>();
        if (pickable != null && !pickables.Contains(pickable))
            pickables.Add(pickable);

        PlaceTarget target = other.GetComponent<PlaceTarget>();
        if (target != null && !placeTargets.Contains(target))
            placeTargets.Add(target);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PickableObject pickable = other.GetComponent<PickableObject>();
        if (pickable != null)
            pickables.Remove(pickable);

        PlaceTarget target = other.GetComponent<PlaceTarget>();
        if (target != null)
            placeTargets.Remove(target);
    }

    // クリック地点に重なる、範囲内の拾えるオブジェクトを返す。なければnull
    public PickableObject GetPickableAtPoint(Vector2 worldPoint)
    {
        for (int i = pickables.Count - 1; i >= 0; i--)
        {
            PickableObject pickable = pickables[i];
            // 破棄済みの参照を掃除する
            if (pickable == null)
            {
                pickables.RemoveAt(i);
                continue;
            }
            if (!pickable.IsPickable) continue;

            Collider2D col = pickable.GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(worldPoint))
                return pickable;
        }
        return null;
    }

    // クリック地点に重なる、まだ埋まっていない設置先を返す。なければnull
    public PlaceTarget GetPlaceTargetAtPoint(Vector2 worldPoint)
    {
        for (int i = placeTargets.Count - 1; i >= 0; i--)
        {
            PlaceTarget target = placeTargets[i];
            if (target == null)
            {
                placeTargets.RemoveAt(i);
                continue;
            }
            if (target.IsFilled) continue;

            Collider2D col = target.GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(worldPoint))
                return target;
        }
        return null;
    }

    // 指定した座標が範囲内かどうかを返す
    public bool IsPointInRange(Vector2 worldPoint)
    {
        return zoneCollider != null && zoneCollider.OverlapPoint(worldPoint);
    }
}