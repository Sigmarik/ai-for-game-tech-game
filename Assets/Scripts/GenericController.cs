using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericController : MonoBehaviour
{
    protected CharacterMovement m_characterMovement;
    protected Interact m_interact;

    private bool m_controlsFrozen = false;

    public static void SetFreezeControls(bool freeze)
    {
        GenericController[] controllers = FindObjectsByType<GenericController>(FindObjectsSortMode.None);

        foreach (GenericController controller in controllers)
        {
            controller.m_controlsFrozen = freeze;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_characterMovement = GetComponent<CharacterMovement>();
        m_interact = GetComponent<Interact>();

        if (m_characterMovement == null) Debug.LogError($"{nameof(PlayerController)} on '{name}' has no {nameof(CharacterMovement)} component.", this);
        if (m_interact == null) Debug.LogError($"{nameof(PlayerController)} on '{name}' has no {nameof(Interact)} component.", this);
    }

    // ===== These are mostly for convenience and to avoid having to get the components in child classes =====

    protected void Move(Vector3 direction)
    {
        if (m_controlsFrozen) return;
        m_characterMovement.Move(direction);
    }

    protected void SetInteracting(bool interacting)
    {
        if (m_controlsFrozen) return;
        m_interact.SetInteracting(interacting);
    }

    protected List<Interactable> FindAllInteractables()
    {
        Interactable[] interactables = FindObjectsOfType<Interactable>();
        return new List<Interactable>(interactables);
    }

    protected List<Interactable> FindAllAvailableInteractables()
    {
        return m_interact.FindAllInteractables();
    }

    protected Interactable FindSelectedInteractable()
    {
        return m_interact.FindClosestInteractable();
    }
}
