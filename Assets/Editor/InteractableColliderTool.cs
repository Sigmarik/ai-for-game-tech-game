using UnityEditor;
using UnityEngine;

public static class InteractableColliderTool
{
    [MenuItem("Tools/Interactables/Add Trigger Colliders to Selection")]
    public static void Run()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("[Interactable Collider Tool] No objects selected. Please select the root object in the Scene or Hierarchy.");
            return;
        }

        int count = 0;

        foreach (var go in selectedObjects)
        {
            // Finds all Interactables on this object and all of its children (including inactive ones)
            var interactables = go.GetComponentsInChildren<Interactable>(true);
            foreach (var interactable in interactables)
            {
                var target = interactable.gameObject;
                
                if (EnsureCollider(target))
                {
                    count++;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        if (count > 0)
        {
            Debug.Log($"[Interactable Collider Tool] Added trigger BoxColliders to {count} object(s).\n" +
                      $"If these objects are prefab instances, remember to Apply the overrides to the prefab assets!");
        }
        else
        {
            Debug.Log("[Interactable Collider Tool] No missing colliders found on the selected Interactable objects.");
        }
    }

    static bool EnsureCollider(GameObject go)
    {
        // Check if the object itself already has a collider
        if (go.GetComponent<BoxCollider>() != null) return false;

        if (!TryGetLocalBounds(go.transform, out Bounds localBounds))
        {
            Debug.LogWarning($"[Interactable Collider Tool] '{go.name}' has no renderers to derive bounds from; skipping.", go);
            return false;
        }

        var box = Undo.AddComponent<BoxCollider>(go);
        box.center    = localBounds.center;
        box.size      = localBounds.size;
        box.isTrigger = true; // Set to not actually collide with anything

        // Ensure the prefab instance registers the new component and its properties
        PrefabUtility.RecordPrefabInstancePropertyModifications(box);
        return true;
    }

    static bool TryGetLocalBounds(Transform target, out Bounds localBounds)
    {
        localBounds = default;

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        bool any = false;

        foreach (var r in target.GetComponentsInChildren<Renderer>(true))
        {
            if (r == null) continue;
            
            // Only consider actual mesh renderers. Excludes UI Canvas, particles, etc.
            if (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)) continue;

            Bounds lb = r.localBounds;
            if (lb.size == Vector3.zero) continue;

            // Transform the renderer's bounds into the target's local space
            Matrix4x4 m = target.worldToLocalMatrix * r.transform.localToWorldMatrix;

            Vector3 c = lb.center, e = lb.extents;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = c + new Vector3(
                    (i & 1) == 0 ? -e.x : e.x,
                    (i & 2) == 0 ? -e.y : e.y,
                    (i & 4) == 0 ? -e.z : e.z);

                Vector3 localCorner = m.MultiplyPoint3x4(corner);
                min = Vector3.Min(min, localCorner);
                max = Vector3.Max(max, localCorner);
            }
            any = true;
        }

        if (!any) return false;

        localBounds = new Bounds((min + max) * 0.5f, max - min);
        return true;
    }
}