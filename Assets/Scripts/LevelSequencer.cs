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
            yield return AdvanceQueueToWorkshop();
            int helperChannel = m_currentCarIndex - m_activeCars.Count + 1;
            yield return MaybeSpawnHelpers(helperChannel);
            yield return new WaitUntil(AreAllActiveCarsFixed);
            yield return WaitForInput("space", 0.5f);
            yield return RemoveAllActiveCars();
        }

        ShowMessage("Instruction", "Congratulations! You fixed all the cars!");
        yield return SpawnConfetti();
    }

    private IEnumerator MaybeSpawnHelpers(int spawnChannel)
    {
        List<AISpawner> spawners = GetSpawnersAtChannel(spawnChannel);
        if (spawners.Count > 0)
        {
            yield return new WaitForSeconds(1.0f);
            foreach (AISpawner spawner in spawners)
            {
                spawner.SetParticlesActive(true);
            }
            SetFreezeControls(true);
            ShowMessage("Instruction", "Help is on the way!");
            FocusCameraOn(spawners[0].gameObject);
            yield return WaitForInput("space", 0.0f);
            foreach (AISpawner spawner in spawners)
            {
                FocusCameraOn(spawner.gameObject);
                yield return new WaitForSeconds(0.5f);
                spawner.SpawnAI();
                SetFreezeControls(true);
                spawner.SetParticlesActive(false);
                yield return new WaitForSeconds(0.5f);
            }
            ShowMessage("Instruction", "Say hi to Riley! She is a mechanic like you!");
            yield return WaitForInput("space");
            ShowMessage("Instruction", "Pretty sure she likes roses and engine oil.");
            yield return WaitForInput("space");
            ShowMessage("Instruction", "She will help you fix the cars faster!");
            yield return WaitForInput("space");
            ShowMessage("Instruction", "");
            ResetCameraFocus();
            SetFreezeControls(false);
        }

        yield return null;
    }
}