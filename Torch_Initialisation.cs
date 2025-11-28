using UnityEngine;
using static UnityEngine.Plugins.SieConfigurableControllerUnity;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;


public class Stage6TorchInit : MonoBehaviour
{
    public Main_Gameplay gameplay;
    public enum Colour
    {
        BLUE, GREEN, YELLOW, ORANGE, RED, INDIGO, VIOLET
    }
    public void BuildTorch()
    {
        Debug.Log("Stage6: Torch initialisation");
        gameplay.SetCubeLights(C2.Led.ALL, Colour.YELLOW, C2.Cube.VibratorB);
        gameplay.SetCubeLights(
            C2.Led.LED0 | C2.Led.LED1 | C2.Led.LED6 | C2.Led.LED7,
            Colour.YELLOW, C2.Cube.TriggerButtonB
        );
    }
}
