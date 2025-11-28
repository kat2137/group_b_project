using UnityEngine;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

public class Stage4CompassNavigation : MonoBehaviour
{
    public Main_Gameplay gameplay;

    private C2.ControllerData sicData;
    private float lastTurnValue = -999f;
    private float deadzone = 3f;   // change only when movement >3 degrees

    // LED masks
    private const C2.Led AllLEDs =
        C2.Led.LED0 | C2.Led.LED1 | C2.Led.LED2 | C2.Led.LED3 |
        C2.Led.LED4 | C2.Led.LED5 | C2.Led.LED6 | C2.Led.LED7;

    private const C2.Led RCross = C2.Led.LED1 | C2.Led.LED2 | C2.Led.LED5 | C2.Led.LED6;
    private const C2.Led LCross = C2.Led.LED0 | C2.Led.LED3 | C2.Led.LED4 | C2.Led.LED7;

    public void ProcessCompass()
    {
        if (C2.SieC2GetControllerData(out sicData) != C2.Result.SIE_C2_OK)
            return;

        float turn = sicData.turnTableA.tableAngle;

        // === NEW SMOOTHING (YOU REQUESTED THIS) ===
        if (Mathf.Abs(turn - lastTurnValue) < deadzone)
            return;

        lastTurnValue = turn;

        // === YOUR ORIGINAL LOGIC, UNCHANGED ===
        if ((turn > 0 && turn < 23) || (turn >= 337 && turn <= 360))
        {
            gameplay.ClearCubeLights(
                C2.Cube.TurnTableA, C2.Cube.ShapesButton, C2.Cube.SpeakerA,
                C2.Cube.VibratorA, C2.Cube.TriggerButtonA);

            gameplay.SetCubeLights(C2.Led.LED0 | C2.Led.LED1, Colour.GREEN, C2.Cube.TurnTableA);
            gameplay.SetCubeLights(AllLEDs, Colour.GREEN, C2.Cube.SpeakerA);
        }
        else if (turn >= 23 && turn < 46)
        {
            gameplay.ClearCubeLights(C2.Cube.TurnTableA);
            gameplay.SetCubeLights(RCross, Colour.GREEN, C2.Cube.TurnTableA);
            gameplay.SetCubeLights(C2.Led.LED4, Colour.GREEN, C2.Cube.SpeakerA);
            gameplay.SetCubeLights(C2.Led.LED0, Colour.GREEN, C2.Cube.VibratorA);
        }
        else if (turn >= 46 && turn < 113)
        {
            //gameplay.ClearCubeLights(gameplay.AllCubes);


            //activate green
            gameplay.SetCubeLights(C2.Led.LED1 | C2.Led.LED3, Colour.GREEN, C2.Cube.TurnTableA);
            gameplay.SetCubeLights(AllLEDs, Colour.GREEN, C2.Cube.VibratorA);

            //activate black
            gameplay.SetCubeLights(C2.Led.LED1 | C2.Led.LED3, Colour.GREEN, C2.Cube.TurnTableA);
        }
        else if (turn >= 113 && turn < 157)
        {
            gameplay.ClearCubeLights(gameplay.AllCubes);
            gameplay.SetCubeLights(LCross, Colour.GREEN, C2.Cube.TurnTableA);
            gameplay.SetCubeLights(C2.Led.LED2, Colour.GREEN, C2.Cube.VibratorA);
            gameplay.SetCubeLights(C2.Led.LED1, Colour.GREEN, C2.Cube.ShapesButton);
        }
        else if (turn >= 157 && turn < 202)
        {
            gameplay.SetCubeLights(C2.Led.LED2 | C2.Led.LED3, Colour.GREEN, C2.Cube.TurnTableA);
            gameplay.SetCubeLights(AllLEDs, Colour.GREEN, C2.Cube.ShapesButton);
        }
        else if (turn >= 202 && turn < 248)
        {
            gameplay.ClearCubeLights(gameplay.AllCubes);
            gameplay.SetCubeLights(RCross, Colour.GREEN, C2.Cube.TurnTableA);
            gameplay.SetCubeLights(C2.Led.LED0, Colour.GREEN, C2.Cube.ShapesButton);
            gameplay.SetCubeLights(C2.Led.LED3, Colour.GREEN, C2.Cube.TriggerButtonA);
        }
        else if (turn >= 248 && turn < 292)
        {
            gameplay.ClearCubeLights(gameplay.AllCubes);
            gameplay.SetCubeLights(C2.Led.LED2 | C2.Led.LED3, Colour.GREEN, C2.Cube.TurnTableA);
            gameplay.SetCubeLights(AllLEDs, Colour.GREEN, C2.Cube.TriggerButtonA);
        }
        else if (turn >= 292 && turn < 337)
        {
            gameplay.ClearCubeLights(gameplay.AllCubes);
            gameplay.SetCubeLights(RCross, Colour.GREEN, C2.Cube.TurnTableA);
            gameplay.SetCubeLights(C2.Led.LED1, Colour.GREEN, C2.Cube.TriggerButtonA);
            gameplay.SetCubeLights(C2.Led.LED3, Colour.GREEN, C2.Cube.SpeakerA);
        }
    }
}
