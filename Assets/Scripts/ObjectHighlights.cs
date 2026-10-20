using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectHighlights
{
    /// <summary>
    /// Sets the highlight state of the specified game object.
    /// </summary>
    /// <param name="obj">The game object to highlight.</param>
    /// <param name="highlight">True to highlight the object, false to remove the highlight.</param>
    /// <param name="color">The color of the highlight.</param>
    /// <param name="thickness">The thickness of the highlight.</param>
    public static void SetHighlight(this GameObject obj, bool highlight, Color? color = null, float? thickness = null)
    {
        var outlineComponent = obj.GetComponent<Outline>();
        if (outlineComponent == null)
        {
            outlineComponent = obj.AddComponent<Outline>();
            outlineComponent.OutlineWidth = 5f;
            outlineComponent.OutlineMode = Outline.Mode.OutlineAll;
        }

        outlineComponent.OutlineColor = color == null ? Color.white : (Color)color;
        if (thickness != null) outlineComponent.OutlineWidth = (float)thickness;
        outlineComponent.enabled = highlight;
    }
}
