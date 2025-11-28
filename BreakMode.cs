using UnityEngine;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

public class Stage2BreakMode : MonoBehaviour
{
    public Main_Gameplay gameplay;
    public enum Colour
    {
        BLUE, GREEN, YELLOW, ORANGE, RED, INDIGO, VIOLET
    }

    [System.Obsolete]
    void Awake()
    {
        gameplay = FindObjectOfType<Main_Gameplay>();
    }

    public void Break()
    {
        Debug.Log("Break Mode: All cubes white");
        gameplay.SetCubeLights(
            C2.Led.ALL, Colour.YELLOW,
            C2.Cube.SpeakerA, C2.Cube.VibratorA, C2.Cube.AnalogStickA, C2.Cube.TriggerButtonA,
            C2.Cube.TurnTableA, C2.Cube.SpeakerB, C2.Cube.VibratorB, C2.Cube.AnalogStickB,
            C2.Cube.TriggerButtonB, C2.Cube.TurnTableB, C2.Cube.DPad, C2.Cube.ShapesButton
        );
    }
}


