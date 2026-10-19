using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSequencer : GenericLevelSequencer
{
    override protected IEnumerator LevelSequence()
    {
        SetFreezeControls(true);
        ShowMessage("Instruction", "This game is about fixing cars!");

        yield return WaitForInput("space");

        ShowMessage("Instruction", "You will be presented with a queue of cars to fix.\nEach car has a problem that you need to solve.");

        yield return new WaitForSeconds(2f);

        yield return PreviewCars();

        SetFreezeControls(false);
        ShowMessage("Instruction", "Good luck!");

        yield return new WaitForSeconds(2f);
        ShowMessage("Instruction", "");

        while (m_currentCarIndex < m_carsToFix.Count)
        {
            yield return AdvanceQueue();
            yield return new WaitUntil(() => m_currentCar != null && IsCarFixed(m_currentCar));
            yield return WaitForInput("space", 0.5f);
            yield return RemoveCurrentCar();
        }

        ShowMessage("Instruction", "Congratulations! You fixed all the cars!");
        yield return SpawnConfetti();
    }
}
