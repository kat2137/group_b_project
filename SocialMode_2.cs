using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

public class SocialMode_2 : MonoBehaviour
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

    private Dictionary<string, Colour> stage4Colours = new();
    private SimpleTimer timer;
    private Dictionary<C2.Cube, CubeLightState> _cubeLightCache = new();

    private const C2.Led RCross = C2.Led.LED1 | C2.Led.LED2 | C2.Led.LED5 | C2.Led.LED6;
    private const C2.Led Lower   = C2.Led.LED4 | C2.Led.LED7;
    private const C2.Led Top     = C2.Led.LED0 | C2.Led.LED5;

    private struct CubeLightState : IEquatable<CubeLightState>
    {
        public C2.Led leds;
        public uint r, g, b;

        public bool Equals(CubeLightState other) =>
            leds == other.leds && r == other.r && g == other.g && b == other.b;
    }

    private class SimpleTimer
    {
        private float t, d;
        private bool running;
        public SimpleTimer(float d) { this.d = d; }
        public void Start() { t = Time.time; running = true; }
        public bool Done() => running && Time.time - t >= d;
    }

    private void Set(C2.Cube cube, C2.Led leds, Colour c)
    {
        var (r, g, b) = ColourValues[c];
        var newState = new CubeLightState { leds = leds, r = r, g = g, b = b };

        if (_cubeLightCache.TryGetValue(cube, out var prev) && prev.Equals(newState))
            return;

        var param = new C2.LedParam { led = leds, red = r, green = g, blue = b };
        C2.SieC2SetLED(cube, in param);
        _cubeLightCache[cube] = newState;
    }

    public void RunSocialMode()
    {
        if (timer == null)
        {
            timer = new SimpleTimer(2f);
            timer.Start();

            stage4Colours["A_R"] = Colour.RED;
            stage4Colours["A_L"] = Colour.ORANGE;
            stage4Colours["A_T"] = Colour.INDIGO;

            stage4Colours["B_R"] = Colour.RED;
            stage4Colours["B_L"] = Colour.YELLOW;
            stage4Colours["B_T"] = Colour.ORANGE;
        }

        if (!timer.Done())
            return;

        // rotate colours
        foreach (var k in stage4Colours.Keys.ToList())
        {
            stage4Colours[k] = Next(stage4Colours[k]);
        }

        // SpeakerA
        Set(C2.Cube.SpeakerA, RCross, stage4Colours["A_R"]);
        Set(C2.Cube.SpeakerA, Lower, stage4Colours["A_L"]);
        Set(C2.Cube.SpeakerA, Top, stage4Colours["A_T"]);

        // ShapesButton (just an example)
        Set(C2.Cube.ShapesButton, RCross, stage4Colours["B_R"]);
        Set(C2.Cube.ShapesButton, Lower, stage4Colours["B_L"]);
        Set(C2.Cube.ShapesButton, Top, stage4Colours["B_T"]);

        timer.Start();
    }

    private Colour Next(Colour current)
    {
        var vals = (Colour[])Enum.GetValues(typeof(Colour));
        int i = Array.IndexOf(vals, current);
        return vals[(i + 1) % vals.Length];
    }

    private void Update()
    {
        RunSocialMode();
    }
}

