using UnityEngine;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

public class Stage5EmergencyMode : MonoBehaviour
{
    public Main_Gameplay gameplay;
    public enum Colour
    {
        BLUE, GREEN, YELLOW, ORANGE, RED, INDIGO, VIOLET
    }
    public void Emergency()
    {
        Debug.Log("Stage5: Emergency mode - flashing red");
        gameplay.SetCubeLights(
            C2.Led.ALL, Colour.RED,
            C2.Cube.SpeakerA, C2.Cube.VibratorA, C2.Cube.AnalogStickA, C2.Cube.TriggerButtonA,
            C2.Cube.TurnTableA, C2.Cube.SpeakerB, C2.Cube.VibratorB, C2.Cube.AnalogStickB,
            C2.Cube.TriggerButtonB, C2.Cube.TurnTableB, C2.Cube.DPad, C2.Cube.ShapesButton
        );
    }
}

