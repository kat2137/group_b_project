using UnityEngine;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

public class Stage7TorchFunction : MonoBehaviour
{
    public Main_Gameplay gameplay;
    private C2.ControllerData data;
    public enum Colour
    {
        BLUE, GREEN, YELLOW, ORANGE, RED, INDIGO, VIOLET
    }
    public void Torch()
    {
        if (C2.SieC2GetControllerData(out data) != C2.Result.SIE_C2_OK) return;

        bool triggerUp   = data.TriggerButtonB.trigger1;
        bool triggerDown = data.TriggerButtonB.trigger;

        if (triggerUp)
        {
            Debug.Log("Torch ON");
            gameplay.SetCubeLights(C2.Led.ALL, Colour.YELLOW,
                C2.Cube.SpeakerA, C2.Cube.VibratorA, C2.Cube.AnalogStickA,
                C2.Cube.TriggerButtonA, C2.Cube.TurnTableA,
                C2.Cube.SpeakerB, C2.Cube.VibratorB, C2.Cube.AnalogStickB,
                C2.Cube.TriggerButtonB, C2.Cube.TurnTableB,
                C2.Cube.DPad, C2.Cube.ShapesButton
            );
        }
        else if (triggerDown)
        {
            Debug.Log("Torch OFF");
            gameplay.ClearCubeLights(
                C2.Cube.SpeakerA, C2.Cube.VibratorA, C2.Cube.AnalogStickA,
                C2.Cube.TriggerButtonA, C2.Cube.TurnTableA,
                C2.Cube.SpeakerB, C2.Cube.VibratorB, C2.Cube.AnalogStickB,
                C2.Cube.TriggerButtonB, C2.Cube.TurnTableB,
                C2.Cube.DPad, C2.Cube.ShapesButton
            );
        }
    }
}

