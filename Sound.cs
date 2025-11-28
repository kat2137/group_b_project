using System.Collections;
using UnityEngine;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

/*
namespace GameLogic;

public class SoundPlayer : MonoBehaviour
{
    
    public void SelectSound(C2.Cube cube, int trackNo, float durationSeconds = 0.0f)
    {
        
        var param = new C2.PlayAudioParam();
        param.trackNo = (uint)trackNo;
        param.repeat = 0;
        C2.SieC2PlayAudio(cube, in param);

        Debug.Log($"Vibrate start: cube={cube}, track={trackNo}, duration={durationSeconds}s");

        if (durationSeconds > 0f)
            StartCoroutine(StopAfter(cube, durationSeconds));
    }


    // Stop vibration immediately
    public void StopSound(C2.Cube cube)
    {
        C2.SieC2StopAudio(cube);
        Debug.Log($"Vibrate stop: cube={cube}");
    }

    private IEnumerator StopAfter(C2.Cube cube, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopVibration(cube);
    }
}
*/