using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;


public class TruthOrDare : MonoBehaviour
{
    public Main_Gameplay gameplay;

    [Tooltip("Seconds the chosen cube stays highlighted")]
    public float highlightDuration = 30f;

    [Tooltip("LED colour used while highlighting (0-100)")]
    public uint ledRed = 100;
    public uint ledGreen = 100;
    public uint ledBlue = 0;

    // fired after each selection (no parameter) — main can read selectedCube field
    public SimpleUnityEvent onEachSelection;

    [HideInInspector]
    public C2.Cube selectedCube = C2.Cube.SpeakerA;

    private Coroutine loopCoroutine;
    private bool stopRequested = false;

    // Start the loop indefinitely
    public void StartLoop()
    {
        StartLoop(-1);
    }

    // Start the loop for a given number of times (times <= 0 => infinite)
    public void StartLoop(int times)
    {
        StopLoop();
        stopRequested = false;
        loopCoroutine = StartCoroutine(SelectionLoop(times));
    }

    // stops the loop (safe to call anytime)
    public void StopLoop()
    {
        stopRequested = true;
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }
        // ensure LEDs cleared when stopping
        ClearAllCubeLEDs();
    }

    private IEnumerator SelectionLoop(int times)
    {
        int remaining = times;
        while (!stopRequested && (remaining > 0 || times <= 0))
        {
            var connected = GetConnectedCubes();
            if (connected.Count == 0)
            {
                Debug.LogWarning("TruthOrDare: no connected cubes found, retrying in 1s.");
                yield return new WaitForSeconds(1f);
                continue;
            }

            int idx = UnityEngine.Random.Range(0, connected.Count);
            selectedCube = connected[idx];
            Debug.Log($"TruthOrDare: selected {selectedCube}");

            // highlight selected cube
            SetCubeLED(selectedCube, ledRed, ledGreen, ledBlue);

            // notify listeners / main gameplay
            onEachSelection?.Invoke();

            // wait highlightDuration or until stopped
            float elapsed = 0f;
            while (elapsed < highlightDuration && !stopRequested)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // clear highlight
            ClearCubeLED(selectedCube);

            if (times > 0) remaining--;

            // small delay between picks
            yield return new WaitForSeconds(0.2f);
        }

        // finished
        loopCoroutine = null;
        Debug.Log("TruthOrDare: loop finished/stopped.");
    }

    // collect connected cubes by querying SDK
    private List<C2.Cube> GetConnectedCubes()
    {
        var list = new List<C2.Cube>();
        for (int i = 0; i < 12; i++)
        {
            C2.Cube cube = (C2.Cube)i;
            C2.ConnectedData cd;
            if (C2.SieC2IsConnected(cube, out cd) == C2.Result.SIE_C2_OK && cd.isConnected)
                list.Add(cube);
        }
        return list;
    }

    // set LED on a single cube
    private void SetCubeLED(C2.Cube cube, uint r, uint g, uint b)
    {
        var param = new C2.LedParam
        {
            led = C2.Led.LED0 | C2.Led.LED1 | C2.Led.LED2 | C2.Led.LED3 |
                  C2.Led.LED4 | C2.Led.LED5 | C2.Led.LED6 | C2.Led.LED7,
            red = r,
            green = g,
            blue = b
        };
        C2.SieC2SetLED(cube, in param);
    }

    // clear LEDs on a single cube
    private void ClearCubeLED(C2.Cube cube)
    {
        var param = new C2.LedParam
        {
            led = C2.Led.LED0 | C2.Led.LED1 | C2.Led.LED2 | C2.Led.LED3 |
                  C2.Led.LED4 | C2.Led.LED5 | C2.Led.LED6 | C2.Led.LED7,
            red = 0,
            green = 0,
            blue = 0
        };
        C2.SieC2SetLED(cube, in param);
    }

    // clear all cubes (0..11)
    private void ClearAllCubeLEDs()
    {
        for (int i = 0; i < 12; i++)
            ClearCubeLED((C2.Cube)i);
    }

    private void OnDisable()
    {
        StopLoop();
    }
}

