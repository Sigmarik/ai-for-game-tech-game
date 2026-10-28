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
    private int uiPresenterCount = 0;
    private int selectorCount = 0;

    public Task ActiveTask { get; private set; }
    public void SetActiveTask(Task task) { if (ActiveTask == null) ActiveTask = task; }
    public void ClearActiveTask(Task task) { if (ActiveTask == task) ActiveTask = null; }

    private MeshRenderer outline;

    private Interact interacter = null;
    public Interact Interacter => interacter;

    private bool active = false;
    public bool IsActive => active;

    [SerializeField] private bool tryCombineChildMeshes = false;
    [SerializeField] private bool splitOutline = false;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        if (!GetComponent<MeshFilter>() && tryCombineChildMeshes)
            Combine();
        if (splitOutline)
        {
            GameObject outlineObject = new GameObject("outline");
            outline = outlineObject.AddComponent<MeshRenderer>();
            outline.materials = GetComponent<MeshRenderer>().materials;
            MeshFilter outlineFilter = outlineObject.AddComponent<MeshFilter>();
            outlineFilter.mesh = Instantiate(GetComponent<MeshFilter>().sharedMesh);
            outlineObject.transform.SetParent(transform, false);
            outlineObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            outlineObject.transform.localPosition = Vector3.zero;
            outlineObject.transform.localScale = Vector3.one;
        }
        else
        {
            outline = GetComponent<MeshRenderer>();
        }
    }

    public void TaskSubscribed()
    {
        taskCount++;
        ApplyDisplayState();
    }

    public void TaskUnsubscribed()
    {
        if (!active) return;
        taskCount--;
        ApplyDisplayState();
    }

    public void AddSelector()
    {
        selectorCount++;
        ApplyDisplayState();
    }

    public void RemoveSelector()
    {
        selectorCount = Mathf.Max(0, selectorCount - 1);
        ApplyDisplayState();
    }

    public bool StartInteraction(Interact p_interacter)
    {
        if (!active) return false;

        if (interacter != null)
        {
            if (interacter == p_interacter) return true;
            return false;
        }

        interacter = p_interacter;

        GenericController controller = p_interacter != null ? p_interacter.GetComponent<GenericController>() : null;
        string interacterType = controller is AIPlayerController ? "ai"
            : controller != null ? "player" : "unknown";

        ExperimentLogger.LogEvent("INTERACTION", "started",
            "interactable", gameObject.name,
            "by", p_interacter != null ? p_interacter.gameObject.name : "null",
            "type", interacterType,
            "parent", transform.root.name);

        ApplyDisplayState();

        OnInteractionStart?.Invoke();
        return true;
    }

    public void StopInteraction()
    {
        if (interacter == null) return;
        Interact oldInteracter = interacter;
        interacter = null;

        GenericController controller = oldInteracter != null ? oldInteracter.GetComponent<GenericController>() : null;
        string interacterType = controller is AIPlayerController ? "ai"
            : controller != null ? "player" : "unknown";

        ExperimentLogger.LogEvent("INTERACTION", "stopped",
            "interactable", gameObject.name,
            "by", oldInteracter != null ? oldInteracter.gameObject.name : "null",
            "type", interacterType,
            "parent", transform.root.name);

        if (OnInteractionEnd != null)
            OnInteractionEnd.Invoke();

        if (oldInteracter != null)
            oldInteracter.OnInteractionStoppedExternally();

        ApplyDisplayState();
    }

    public void NextState()
    {
        if (!active) return;
    }

    static Color HINT_COLOR = new Color(251f / 255f, 126f / 255f, 43f / 255f);

    public void ForceUpdate()
    {
        if (!active) return;
        ApplyDisplayState();
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
        uiPresenterCount++;
        ApplyDisplayState();
    }

    public void HideUI()
    {
        uiPresenterCount = Mathf.Max(0, uiPresenterCount - 1);
        ApplyDisplayState();
    }

    private void ApplyDisplayState()
    {
        if (!active)
        {
            uiCanvas.SetActive(false);
            SetHighlight(false);
            return;
        }

        bool hasTasks = taskCount > 0;
        bool interactionActive = interacter != null;
        bool selected = hasTasks && (selectorCount > 0 || interactionActive);
        bool available = hasTasks && interacter == null;

        if (selected)
        {
            SetHighlight(true, Color.white, 5f);
        }
        else if (available)
        {
            SetHighlight(true, HINT_COLOR, 3f);
        }
        else
        {
            SetHighlight(false);
        }

        uiCanvas.SetActive(hasTasks && (interactionActive || uiPresenterCount > 0));
    }

    public void SetHighlight(bool on, Color? color = null, float? thickness = null)
    {
        if (!active || !gameObject.active) return;
        outline.gameObject.SetHighlight(on, color, thickness);
    }

    public void SetActive(bool p_active)
    {
        if (active == p_active) return;

        if (!p_active)
        {
            StopInteraction();
            uiPresenterCount = 0;
            selectorCount = 0;
            uiCanvas.SetActive(false);
            SetHighlight(false);
        }

        active = p_active;

        if (splitOutline)
            outline.gameObject.SetActive(p_active);

        if (active)
        {
            ApplyDisplayState();
        }
    }

    public void Combine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        Dictionary<Material, List<CombineInstance>> groups = new Dictionary<Material, List<CombineInstance>>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.gameObject == gameObject) continue;
            if (meshFilter.sharedMesh == null) continue;

            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            if (meshRenderer == null) continue;

            Material[] materials = meshRenderer.sharedMaterials;
            Mesh mesh = meshFilter.sharedMesh;

            Matrix4x4 matrix = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;

            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
            {
                if (subMeshIndex >= materials.Length || materials[subMeshIndex] == null) continue;

                Material material = materials[subMeshIndex];

                if (!groups.ContainsKey(material))
                    groups[material] = new List<CombineInstance>();

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = mesh;
                combineInstance.subMeshIndex = subMeshIndex;
                combineInstance.transform = matrix;

                groups[material].Add(combineInstance);
            }
        }

        if (groups.Count == 0) return;

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = gameObject.name + "_CombinedMesh";
        combinedMesh.indexFormat = IndexFormat.UInt32;

        List<Material> combinedMaterials = new List<Material>();
        List<CombineInstance> materialMeshes = new List<CombineInstance>();

        foreach (var group in groups)
        {
            Mesh materialMesh = new Mesh();
            materialMesh.indexFormat = IndexFormat.UInt32;

            materialMesh.CombineMeshes(group.Value.ToArray(), true, true);

            CombineInstance combineInstance = new CombineInstance();
            combineInstance.mesh = materialMesh;
            combineInstance.subMeshIndex = 0;
            combineInstance.transform = Matrix4x4.identity;

            materialMeshes.Add(combineInstance);
            combinedMaterials.Add(group.Key);
        }

        combinedMesh.CombineMeshes(materialMeshes.ToArray(), false, false);

        MeshFilter parentFilter = GetComponent<MeshFilter>();
        if (parentFilter == null) parentFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer parentRenderer = GetComponent<MeshRenderer>();
        if (parentRenderer == null) parentRenderer = gameObject.AddComponent<MeshRenderer>();

        parentFilter.sharedMesh = combinedMesh;
        parentRenderer.sharedMaterials = combinedMaterials.ToArray();
    }
}