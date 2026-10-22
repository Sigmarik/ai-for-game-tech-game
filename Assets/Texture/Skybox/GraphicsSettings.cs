
using UnityEngine;
using TMPro;

public class GraphicsSettings : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown;

    void Start()
    {
        // Match dropdown to the current Unity quality.
        graphicsDropdown.SetValueWithoutNotify(
            QualitySettings.GetQualityLevel()
        );

        graphicsDropdown.RefreshShownValue();
    }

    public void ChangeGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);

        Debug.Log("Graphics Quality: " +
                  QualitySettings.names[qualityIndex]);
    }
}
