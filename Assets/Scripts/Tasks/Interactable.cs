using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Interactable : MonoBehaviour
{
    public event Action OnInteractionStart;
    public event Action OnInteractionEnd;
    public event Action CheckTasks;

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameObject uiCanvas;

    private int taskCount = 0;

    public Task ActiveTask {get; private set;} // add this so Interactable will know which task is active
    public void SetActiveTask(Task task) {ActiveTask = task;}
    public void ClearActiveTask(Task task) {if(ActiveTask == task) ActiveTask  = null; }

    private MeshRenderer outline;

    private Interact interacter = null;

    private bool active = false;
    public bool IsActive => active;

    [SerializeField] private bool tryCombineChildMeshes = false;
    [SerializeField] private bool splitOutline = false;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        if (!GetComponent<MeshFilter>() && tryCombineChildMeshes)
            Combine();
        if(splitOutline)
        {
            GameObject obj = new GameObject("outline");
            outline = obj.AddComponent<MeshRenderer>();
            outline.materials = GetComponent<MeshRenderer>().materials;
            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.mesh = Instantiate(GetComponent<MeshFilter>().sharedMesh);
            obj.transform.SetParent(transform, false);
            obj.transform.localRotation = Quaternion.Euler(0, 0, 0); 
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
        }
        else
        {
            outline = GetComponent<MeshRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        if (taskCount <= 0)
            uiCanvas.SetActive(false);
    }

    public void TaskSubscribed()
    {
        taskCount++;
    }

    public void TaskUnsubscribed()
    {
        if (!active) return;
        taskCount--;
        if (taskCount <= 0)
        {
            uiCanvas.SetActive(false);
            SetHighlight(false);
        }
    }

    public bool StartInteraction(Interact p_interacter)
    {
        if (!active || interacter != null) return false;
        interacter = p_interacter;
        OnInteractionStart?.Invoke();
        if (ActiveTask == null)
        {
            interacter = null;
            return false;
        }
        return true;
    }

    public void StopInteraction()
    {
        if (!active) return;
        interacter = null;
        if (OnInteractionEnd != null)
            OnInteractionEnd.Invoke();
    }

    public void NextState()
    {
        if (!active)
            return;
    }

    static Color HINT_COLOR = new Color(251f / 255f, 126f / 255f, 43f / 255f);

    public void ForceUpdate()
    {
        if (!active)
            return;
        if (interacter != null && interacter.IsInteractable(this))
        {
            SetHighlight(true, Color.white, 5f);
            ShowUI();
        }
        else
        {
            SetHighlight(true, HINT_COLOR, 3f);
            HideUI();
        }
    }

    public void StartProgressBar(float time)
    {
        if (!active) return;
        progressBar.StartProgressBar(time);
    }
    public void StopProgressBar()
    {
        if (!active) return;
        progressBar.StopProgressBar();
    }
    public void ShowUI()
    {
        if (!active) return;
        Debug.Log($"{name} showing UI - true");
        if (taskCount <= 0)
            return;
        uiCanvas.SetActive(true);
        SetHighlight(true, Color.white, 5f);
    }
    public void HideUI()
    {
        if (!active) return;
        Debug.Log($"{name} showing UI - false");
        uiCanvas.SetActive(false);
        SetHighlight(true, HINT_COLOR, 3f);
    }

    public void SetHighlight(bool on,  Color? color = null, float? thickness = null)
    {
        if (!active) return;
            outline.gameObject.SetHighlight(on, color, thickness);
        
    }

    public void SetActive(bool p_active)
    {
        Debug.Log($"Set interactable {name} to: {p_active}");
        active = p_active;
        if(splitOutline)
            outline.gameObject.SetActive(p_active);
        SetHighlight(active, HINT_COLOR, 3f);
    }

    public void Combine()
    {
        MeshFilter[] meshFilters =
            GetComponentsInChildren<MeshFilter>();

        // Group meshes by material.
        Dictionary<Material, List<CombineInstance>> groups =
            new Dictionary<Material, List<CombineInstance>>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject == gameObject)
                continue;

            if (mf.sharedMesh == null)
                continue;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();

            if (mr == null)
                continue;

            Material[] materials = mr.sharedMaterials;
            Mesh mesh = mf.sharedMesh;

            Matrix4x4 matrix =
                transform.worldToLocalMatrix *
                mf.transform.localToWorldMatrix;

            for (int sub = 0; sub < mesh.subMeshCount; sub++)
            {
                if (sub >= materials.Length ||
                    materials[sub] == null)
                    continue;

                Material material = materials[sub];

                if (!groups.ContainsKey(material))
                    groups[material] = new List<CombineInstance>();

                CombineInstance ci = new CombineInstance();
                ci.mesh = mesh;
                ci.subMeshIndex = sub;
                ci.transform = matrix;

                groups[material].Add(ci);
            }
        }

        if (groups.Count == 0)
        {
            Debug.LogWarning("No valid meshes/materials found.");
            return;
        }

        // Create one combined mesh with one submesh per material.
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = gameObject.name + "_CombinedMesh";

        combinedMesh.indexFormat = IndexFormat.UInt32;

        List<Material> combinedMaterials =
            new List<Material>();

        List<CombineInstance> materialMeshes =
            new List<CombineInstance>();

        foreach (var group in groups)
        {
            Mesh materialMesh = new Mesh();
            materialMesh.indexFormat = IndexFormat.UInt32;

            materialMesh.CombineMeshes(
                group.Value.ToArray(),
                true,
                true
            );

            CombineInstance ci = new CombineInstance();
            ci.mesh = materialMesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;

            materialMeshes.Add(ci);
            combinedMaterials.Add(group.Key);
        }

        combinedMesh.CombineMeshes(
            materialMeshes.ToArray(),
            false, // Keep separate submeshes
            false
        );

        // Assign the combined mesh and all materials to the parent.
        MeshFilter parentFilter = GetComponent<MeshFilter>();

        if (parentFilter == null)
            parentFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer parentRenderer = GetComponent<MeshRenderer>();

        if (parentRenderer == null)
            parentRenderer = gameObject.AddComponent<MeshRenderer>();

        parentFilter.sharedMesh = combinedMesh;
        parentRenderer.sharedMaterials = combinedMaterials.ToArray();

        // Remove child renderers and mesh filters.
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject == gameObject)
                continue;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();

            if (mr != null)
                mr.enabled = false;
        }

        Debug.Log(
            $"Combined meshes into {combinedMaterials.Count} material submeshes."
        );
    }
}
