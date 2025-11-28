
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

//GAME MODE

public class Full_GameMode : MonoBehaviour
{
    public Main_Gameplay gameplay;
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
    public enum State
    {
        None,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6,
        Stage7,
        Completed
    }
    // C2 cubes
    private C2.ControllerData sicData;
    private C2.Cube cube0, cube1, cube2, cube3, cube4, cube5, cube6, cube7, cube8, cube9, cube10, cube11;
    private Cube[] cubeList = new Cube[12];
    //external game objects
    public GameObject treasureChest;
    public GameObject treasureChestPrefab;
    public GameObject scoreDisplayPrefab;
    private int score;

    //random cube selection
    var random = new Random();
    int index = random.Next(0, cubes.Count);
    Cube selectedCube = cubes[index];
    // fetching sound player
    public SoundPlayer soundPlayer;

    private State currentState = State.Stage1;
    private State previousState = State.None;

    private Dictionary<C2.Cube, CubeLightState> _cubeLightCache = new();
    //timer debounce
    private float debounceDelay = 1f;
    private float lastInputTime;
    private bool WasQuitButtonPressed;

    private SimpleTimer stageTimer;
    private SimpleTimer stage4Timer;
    private Dictionary<string, Colour> stage4Colours = new();

    private class SimpleTimer
    {
        private float startTime;
        private float duration;
        private bool running;

        public SimpleTimer(float duration) { this.duration = duration; }

        public void Start() { startTime = Time.time; running = true; }

        public bool HasElapsed() => running && (Time.time - startTime >= duration);

        public void Reset() { running = false; }
    }

    // === LIGHT STATE STRUCT ===
    private struct CubeLightState : IEquatable<CubeLightState>
    {
        public C2.Led leds;
        public uint red, green, blue;

        public bool Equals(CubeLightState other) =>
            leds == other.leds && red == other.red && green == other.green && blue == other.blue;
        public override int GetHashCode() => HashCode.Combine(leds, red, green, blue);
    }
    private struct Cube
    {
        public C2.Cube cube;
        public bool connected;
    }


    private static Colour GetNextColour(Colour current)
    {
        var values = (Colour[])Enum.GetValues(typeof(Colour));
        int index = Array.IndexOf(values, current);
        return values[(index + 1) % values.Length];
    }

    private void SetCubeLights(C2.Led leds, Colour colour, params C2.Cube[] cubes)
    {
        var (r, g, b) = ColourValues[colour];
        var newState = new CubeLightState { leds = leds, red = r, green = g, blue = b };
        var lights = new C2.LedParam { led = leds, red = r, green = g, blue = b };

        foreach (var cube in cubes)
        {
            if (_cubeLightCache.TryGetValue(cube, out var prev) && prev.Equals(newState)) continue;
            C2.SieC2SetLED(cube, in lights);
            _cubeLightCache[cube] = newState;
        }
    }

    // overload to set by raw rgb values
    private void SetCubeLights(C2.Led leds, uint r, uint g, uint b, params C2.Cube[] cubes)
    {
        var newState = new CubeLightState { leds = leds, red = r, green = g, blue = b };
        var lights = new C2.LedParam { led = leds, red = r, green = g, blue = b };
        foreach (var cube in cubes)
        {
            if (_cubeLightCache.TryGetValue(cube, out var prev) && prev.Equals(newState)) continue;
            C2.SieC2SetLED(cube, in lights);
            _cubeLightCache[cube] = newState;
        }
    }

    private void ClearCubeLights(params C2.Cube[] cubes)
    {
        var lights = new C2.LedParam { led = AllLEDs, red = 0, green = 0, blue = 0 };
        foreach (var cube in cubes)
        {
            C2.SieC2SetLED(cube, in lights);
            _cubeLightCache[cube] = new CubeLightState { leds = AllLEDs, red = 0, green = 0, blue = 0 };
        }
    }

    private const C2.Led AllLEDs = C2.Led.LED0 | C2.Led.LED1 | C2.Led.LED2 | C2.Led.LED3 |
                                   C2.Led.LED4 | C2.Led.LED5 | C2.Led.LED6 | C2.Led.LED7;
    private const C2.Led RCross = C2.Led.LED1 | C2.Led.LED2 | C2.Led.LED5 | C2.Led.LED6;
    private const C2.Led LCross = C2.Led.LED0 | C2.Led.LED3 | C2.Led.LED4 | C2.Led.LED7;

    public State CurrentState = State.None;
    private void HandleStateLogic()
    {
        switch (currentState)
        {
            case State.Stage1:
                Debug.Log("Stage1: Game starts");
                SelectSound(C2.Cube.ALL, 100, 3f);
                break;
            case State.Stage2:
                Debug.Log("Stage2: Role Assignment");

                Debug.Log($"Explorer chosen: {explorerCube}");
                SetCubeLights(AllLEDs, Colour.YELLOW, explorerCube);
                SelectSound(explorerCube, 150, 3f);

                foreach (var cube in connectedCubes)
                {
                if (cube != explorerCube)
                SetCubeLights(AllLEDs, Colour.BLUE, cube);
                }
                break;
            case State.Stage3:
                Debug.Log("Stage3: Treasure Hunt begins");
                SetCubeLights(AllLEDs, Colour.YELLOW, cube0, cube1, cube2, cube3, cube4, cube5, cube6, cube7, cube8, cube9, cube10, cube11);
                if (treasureChest != null)
                {
                    treasureChest.SetActive(true);
                    Debug.Log("Treasure chest activated in the game world.");

                    var coll = treasureChest.GetComponent<GameLogic.TreasureHunt.Collectible>();
                    coll ??= treasureChest.AddComponent<GameLogic.TreasureHunt.Collectible>();
                    coll.soundPlayer = soundPlayer;
                    coll.onCollected = coll.onCollected ?? new UnityEngine.Events.UnityEvent();
                    coll.onCollected.RemoveAllListeners();
                    coll.onCollected.AddListener(() => CollectTreasure(explorerCube));
                }
                break;
            case State.Stage4:
                break;
            case State.Stage5:
                Debug.Log("Stage5: Treasure Collected");
                break;
                
        }
    
    }

    void Start()
    {
        compassComp = gameObject.GetComponent<Compass_Main>() ?? gameObject.AddComponent<Compass_Main>();

        cube0 = C2.Cube.SpeakerA;
        cube1 = C2.Cube.VibratorA;
        cube2 = C2.Cube.AnalogStickA;
        cube3 = C2.Cube.TriggerButtonA;
        cube4 = C2.Cube.TurnTableA;
        cube5 = C2.Cube.SpeakerB;
        cube6 = C2.Cube.VibratorB;
        cube7 = C2.Cube.AnalogStickB;
        cube8 = C2.Cube.TriggerButtonB;
        cube9 = C2.Cube.TurnTableB;
        cube10 = C2.Cube.DPad;
        cube11 = C2.Cube.ShapesButton;
        score = 0;
        for (int i = 0; i < cubeList.Length; i++)
        {
            cubeList[i].cube = (C2.Cube)i;
            cubeList[i].connected = false;
        }
        //activating treasure hunt objects
        if (treasureChest != null) treasureChest.SetActive(false);

        var startParam = new C2.ControllerStartParam { intervalMilliseconds = 100 };
        //picking explorer cube by random choice
        List<C2.Cube> connectedCubes = new List<C2.Cube>();
                for (int i = 0; i < cubeList.Length; i++)
                {
                    if (cubeList[i].connected)
                     connectedCubes.Add(cubeList[i].cube);
                }
                if (connectedCubes.Count == 0)
                {
                     Debug.LogWarning("No connected cubes found for role assignment!");
                    break;
                }
                System.Random rng = new System.Random();
                int index = rng.Next(connectedCubes.Count);
                C2.Cube explorerCube = connectedCubes[index];
        void Update()
        {
            HandleStateLogic();
            //game code comes here
        }
    }
}


*/