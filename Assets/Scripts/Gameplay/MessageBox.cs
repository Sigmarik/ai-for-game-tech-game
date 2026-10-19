using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    private bool m_isPulsing = false;

    [SerializeField] public string channel = "Default";

    public void DisplayMessage(string message, bool pulse = false)
    {
        m_isPulsing = pulse;
        if (TryGetComponent<TMPro.TextMeshProUGUI>(out var uiTmp))
        {
            uiTmp.text = message;
        }
        else if (TryGetComponent<TMPro.TextMeshPro>(out var tmp))
        {
            tmp.text = message;
        }
        else
        {
            Debug.LogWarning($"MessageBox on '{name}' has no TMP component (on self or children).", this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPulsing)
        {
            float pulse = Mathf.Sin(Time.time * 10f) * 0.05f + 1.0f;
            transform.localScale = new Vector3(pulse, pulse, pulse);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}
