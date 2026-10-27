using UnityEngine;

public class HoodController : MonoBehaviour
{
    [SerializeField] private Transform hoodPivot;
    [SerializeField] private Vector3 openRotationOffset = new Vector3(-65f, 0f, 0f);
    [SerializeField, Min(1f)] private float degreesPerSecond = 100f;
    private Quaternion closedRotation;
    private bool initialized;
    private bool wantsOpen;
    public bool IsOpen { get; private set; }

    private bool Initialize()
    {
        if (initialized) return hoodPivot != null;
        if (hoodPivot == null) return false;
        closedRotation = hoodPivot.localRotation;
        initialized = true;
        return true;
    }

    private void Awake() { Initialize(); }

    private void Update()
    {
        if (!Initialize()) return;
        Quaternion target = wantsOpen
            ? closedRotation * Quaternion.Euler(openRotationOffset)
            : closedRotation;
        hoodPivot.localRotation = Quaternion.RotateTowards(
            hoodPivot.localRotation, target, degreesPerSecond * Time.deltaTime);
        IsOpen = wantsOpen && Quaternion.Angle(hoodPivot.localRotation, target) < 0.1f;
    }

    [ContextMenu("Open Hood (Play Mode)")]
    public void OpenHood()
    {
        if (!Application.isPlaying) return;
        if (!Initialize())
        {
            Debug.LogError("Assign HoodPivot in the Inspector.", this);
            return;
        }
        wantsOpen = true;
    }

    [ContextMenu("Close Hood (Play Mode)")]
    public void CloseHood()
    {
        if (!Application.isPlaying) return;
        wantsOpen = false;
        IsOpen = false;
    }
}
