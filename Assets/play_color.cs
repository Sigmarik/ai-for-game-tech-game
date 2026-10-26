using UnityEngine;
using TMPro;

public class TextColor : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.color = Color.white;
    }
}