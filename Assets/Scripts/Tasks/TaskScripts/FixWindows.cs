using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FixWindows : Task
{
    [SerializeField] private Renderer windowRenderer;
    [SerializeField] private float repairEffectDuration = 3f;
    private static readonly int BreakAmountId = Shader.PropertyToID("_BreakAmount");

    public override void OnTaskEnd(Task task)
    {
        SetRepaired();
    }
    void SetRepaired()
    {
        foreach(var mat in windowRenderer.materials)
        {
            if(mat.HasProperty(BreakAmountId))
                DOVirtual.Float(1, 0f, repairEffectDuration, v => mat.SetFloat(BreakAmountId, v));
        }
    }
}
