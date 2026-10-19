using TMPro;
using UnityEngine;

/// <summary>
/// Resizes this panel to tightly wrap its child TMP_Text content, and visually
/// hides the panel when the text is empty. Attach to the panel that has a
/// single TextMeshProUGUI child.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class StretchToContain : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Child text to fit around. Auto-found in children if left empty.")]
    [SerializeField] private TMP_Text targetText;

    [Header("Sizing")]
    [Tooltip("Extra space around the text inside the panel.")]
    [SerializeField] private Vector2 padding = new Vector2(24f, 12f);

    [Tooltip("If > 0, text wraps at this width and the panel grows vertically. " +
             "Leave at 0 to let the text be as wide as it needs (no wrapping).")]
    [SerializeField] private float maxWidth = 0f;

    private RectTransform panelRect;
    private CanvasGroup canvasGroup;
    private string lastText;

    void Awake()
    {
        panelRect = GetComponent<RectTransform>();

        // CanvasGroup lets us "hide" the panel while keeping the script alive so
        // it can still detect when new text arrives.
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>(true);
    }

    void OnEnable() => Refresh();

    // Cheap polling: TMP has no direct "on text changed" event for a field.
    void Update()
    {
        if (targetText != null && targetText.text != lastText)
            Refresh();
    }

    /// <summary>Manually re-evaluate size / visibility. Call after setting text.</summary>
    public void Refresh()
    {
        if (targetText == null) return;

        lastText = targetText.text;

        // Constrain text width so TMP can wrap, and give it tall height so
        // nothing gets clipped during the measuring pass.
        if (maxWidth > 0f)
            targetText.rectTransform.sizeDelta = new Vector2(maxWidth, 10000f);

        targetText.ForceMeshUpdate();

        // characterCount is more reliable than string.IsNullOrEmpty for
        // strings that contain only rich-text tags / whitespace.
        bool isEmpty = targetText.textInfo.characterCount == 0;

        SetVisible(!isEmpty);
        if (isEmpty) return;

        // preferredWidth/Height reflect the wrapped text metrics.
        float w = targetText.preferredWidth;
        float h = targetText.preferredHeight;

        panelRect.sizeDelta = new Vector2(w + padding.x, h + padding.y);

        RectTransform textRect = targetText.rectTransform;
        textRect.sizeDelta = new Vector2(w, h);
        textRect.anchoredPosition = Vector2.zero;
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha          = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable   = visible;
    }
}