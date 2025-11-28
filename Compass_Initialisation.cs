using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Plugins.SieConfigurableControllerUnity;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

public class Stage1CompassSetup : MonoBehaviour
{

    public Main_Gameplay gameplay;

    void Start()
    {
        Debug.Log("Stage1: Compass setup (with 5-second light sequence)");

        C2.ControllerStartParam controllerStartParam;
        controllerStartParam.intervalMilliseconds = 100;
        C2.Result result = C2.SieC2StartControllerData(controllerStartParam);
        if (result == C2.Result.SIE_C2_OK)
        {
            Debug.Log($"Start Controller Data: result={result}, interval={100}");
        }
        else
        {
            Debug.LogError($"Error:: Start ContC2roller Data: result={result}, interval={100}");
        }


        StartCoroutine(RunStage(5f));
    }

    private IEnumerator RunStage(float timeToWait)
    {
        // Clear all lights first
        gameplay.ClearCubeLights(
            C2.Cube.TurnTableA,
            C2.Cube.ShapesButton,
            C2.Cube.SpeakerA,
            C2.Cube.VibratorA,
            C2.Cube.TriggerButtonA
        );

        // === Pair 1 ===
        gameplay.SetCubeLights(C2.Led.LED2 | C2.Led.LED3 | C2.Led.LED6 | C2.Led.LED7, Colour.YELLOW, C2.Cube.TurnTableA);
        gameplay.SetCubeLights(
            C2.Led.LED0 | C2.Led.LED1 | C2.Led.LED4 | C2.Led.LED5,
            Colour.YELLOW,
            C2.Cube.ShapesButton
        );
        yield return new WaitForSeconds(timeToWait);

        // === Pair 2 ===
        gameplay.SetCubeLights(C2.Led.LED2 | C2.Led.LED3 | C2.Led.LED6 | C2.Led.LED7, Colour.YELLOW, C2.Cube.SpeakerA);
        gameplay.SetCubeLights(C2.Led.LED0 | C2.Led.LED1 | C2.Led.LED4 | C2.Led.LED5, Colour.YELLOW, C2.Cube.TurnTableA);
        yield return new WaitForSeconds(timeToWait);

        // === Pair 3 ===
        gameplay.SetCubeLights(C2.Led.LED0 | C2.Led.LED2 | C2.Led.LED4 | C2.Led.LED6, Colour.YELLOW, C2.Cube.VibratorA);
        gameplay.SetCubeLights(C2.Led.LED1 | C2.Led.LED3 | C2.Led.LED5 | C2.Led.LED7, Colour.YELLOW, C2.Cube.TurnTableA);
        yield return new WaitForSeconds(timeToWait);

        // === Pair 4 ===
        gameplay.SetCubeLights(C2.Led.LED1 | C2.Led.LED3 | C2.Led.LED5 | C2.Led.LED7, Colour.YELLOW, C2.Cube.TriggerButtonA);
        gameplay.SetCubeLights(C2.Led.LED0 | C2.Led.LED2 | C2.Led.LED4 | C2.Led.LED6, Colour.YELLOW, C2.Cube.TurnTableA);
        yield return new WaitForSeconds(timeToWait);

        Debug.Log("Stage1: Compass setup complete.");

        SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
    }
}
