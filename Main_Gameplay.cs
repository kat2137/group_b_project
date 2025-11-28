using System;
using System.Collections.Generic;
using UnityEngine;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

public class Main_Gameplay : MonoBehaviour
{
    public C2.Cube[] AllCubes = (C2.Cube[])Enum.GetValues(typeof(C2.Cube));
    public enum Colour
    {
        BLUE, GREEN, YELLOW, ORANGE, RED, INDIGO, VIOLET
    }
    private static readonly Dictionary<Colour, (uint R, uint G, uint B)> ColourValues = new()
    {
        { Colour.BLUE,   (0,   0, 100) },
        { Colour.GREEN,  (0, 100,   0) },
        { Colour.YELLOW, (100,100,  0) },
        { Colour.ORANGE, (100,65,   0) },
        { Colour.RED,    (100, 0,   0) },
        { Colour.INDIGO, (29,  0,  51) },
        { Colour.VIOLET, (58,  0,  82) }
    };

    public Colour NextColour(Colour current)
    {
        var values = (Colour[])Enum.GetValues(typeof(Colour));
        int index = Array.IndexOf(values, current);
        return values[(index + 1) % values.Length];
    }

    public void SetCubeLights(C2.Led leds, Colour colour, params C2.Cube[] cubes)
    {
        var (r, g, b) = ColourValues[colour];
        SetCubeLights(leds, r, g, b, cubes);
    }

    public void SetCubeLights(C2.Led leds, uint r, uint g, uint b, params C2.Cube[] cubes)
    {
        var lights = new C2.LedParam { led = leds, red = r, green = g, blue = b };
        foreach (var cube in cubes)
            C2.SieC2SetLED(cube, in lights);
    }

    public void ClearCubeLights(params C2.Cube[] cubes)
    {
        var off = new C2.LedParam { led = C2.Led.ALL, red = 0, green = 0, blue = 0 };
        foreach (var cube in cubes)
            C2.SieC2SetLED(cube, in off);
    }

    public void ClearAllCubeLights()
    {
        for (int i = 0; i < 12; i++)
            ClearCubeLights((C2.Cube)i);
    }

   
    private bool breakModeActive = false;
    private bool emergencyActive = false;
    private bool truthOrDareActive = false;

    private bool IsAnyModeActive()
    {
        return breakModeActive || emergencyActive || truthOrDareActive || socialMode.enabled;
    }

    
    private Stage1CompassSetup compassSetup;
    private Stage4CompassNavigation compassNavigation;
    private Stage2BreakMode breakMode;
    private SocialMode_2 socialMode;
    private Stage5EmergencyMode emergencyMode;
    private Stage7TorchFunction torchFunction;
    private TruthOrDare truthOrDare;
    private SoundPlayer soundPlayer;

    private C2.ControllerData sicData;
    private float debounce = 0.5f;
    private float lastPressTime;


    void Awake()
    {
        
        compassSetup      = gameObject.AddComponent<Stage1CompassSetup>();
        compassNavigation = gameObject.AddComponent<Stage4CompassNavigation>();
        breakMode         = gameObject.AddComponent<Stage2BreakMode>();
        socialMode        = gameObject.AddComponent<SocialMode_2>();
        emergencyMode     = gameObject.AddComponent<Stage5EmergencyMode>();
        torchFunction     = gameObject.AddComponent<Stage7TorchFunction>();
        truthOrDare       = gameObject.AddComponent<TruthOrDare>();
        soundPlayer       = gameObject.AddComponent<SoundPlayer>();

        
        compassSetup.gameplay      = this;
        compassNavigation.gameplay = this;
        breakMode.gameplay         = this;
        socialMode.gameplay        = this;
        emergencyMode.gameplay     = this;
        torchFunction.gameplay     = this;
        truthOrDare.gameplay       = this;
        soundPlayer.gameplay       = this;

        
        socialMode.enabled = false;

        Debug.Log("Main Gameplay initialised.");
    }

    void Start()
    {   
        C2.ControllerStartParam param;
        param.intervalMilliseconds = 100;

        var result = C2.SieC2StartControllerData(param);

        if (result == C2.Result.SIE_C2_OK)
            Debug.Log("C2 controller data stream started successfully.");
        else
            Debug.LogError("Failed starting C2 controller data stream: " + result);

        Debug.Log("Main Gameplay initialised.");

    }
        void Update()
    {
        if (C2.SieC2GetControllerData(out sicData) != C2.Result.SIE_C2_OK)
            return;

        
        torchFunction.Torch();

        compassNavigation.enabled = !IsAnyModeActive();

        if (Time.time - lastPressTime < debounce)
            return;


        if (sicData.buttons.square)
        {
            breakModeActive = true;
            emergencyActive = false;
            truthOrDareActive = false;
            socialMode.enabled = false;

            breakMode.Break();
            lastPressTime = Time.time;
        }

        else if (sicData.buttons.circle)
        {
            socialMode.enabled = true;
            breakModeActive = false;
            emergencyActive = false;
            truthOrDareActive = false;

            lastPressTime = Time.time;
        }

       
        else if (sicData.buttons.cross)
        {
            socialMode.enabled = false;
            breakModeActive = false;
            emergencyActive = false;
            truthOrDareActive = false;

            ClearAllCubeLights();
            lastPressTime = Time.time;
        }

        
        else if (sicData.buttons.triangle)
        {
            emergencyActive = true;
            socialMode.enabled = false;
            breakModeActive = false;
            truthOrDareActive = false;

            emergencyMode.Emergency();
            lastPressTime = Time.time;
        }

       
        else if (sicData.AnalogStickA.button)
        {
            truthOrDareActive = true;
            breakModeActive = false;
            emergencyActive = false;
            socialMode.enabled = false;

            truthOrDare.StartLoop(2);
            soundPlayer.SelectSound(C2.Cube.SpeakerA, 7, 1.5f);
            lastPressTime = Time.time;
        }
    }
}


