using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using EventInstance = FMOD.Studio.EventInstance;
using RuntimeManager = FMODUnity.RuntimeManager;



public class SoundManager : MonoBehaviour
{
    [EventRef]
    public string blocked;
    [EventRef]
    public string bothDestroyed;
    [EventRef]
    public string countdown;
    [EventRef]
    public string evolve;
    [EventRef]
    public string gameStart;
    [EventRef]
    public string invocation;
    [EventRef]
    public string kill;
    [EventRef]
    public string music;
    [EventRef]
    public string nfc_detect_left;
    [EventRef]
    public string nfc_detect_right;
    [EventRef]
    public string baseDamage;

    private EventInstance blockedInstance;
    private EventInstance bothDestroyedInstance;
    private EventInstance countdownInstance;
    private EventInstance gameStartInstance;
    private EventInstance invocationInstance;
    private EventInstance killInstance;
    private EventInstance musicInstance;
    private EventInstance nfc_detect_leftInstance;
    private EventInstance nfc_detect_rightInstance;
    private EventInstance baseDamageInstance;

    private EventInstance evolveP1;
    private EventInstance evolveP2;
    private Coroutine p1EvolveRoutine;
    private Coroutine p2EvolveRoutine;

    public enum Player
    { P1, P2};

    public static SoundManager instance;

    void Start()
    {
        if (instance != null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        musicInstance = RuntimeManager.CreateInstance(music);
        gameStartInstance = RuntimeManager.CreateInstance(gameStart);
        nfc_detect_leftInstance = RuntimeManager.CreateInstance(nfc_detect_left);
        nfc_detect_rightInstance = RuntimeManager.CreateInstance(nfc_detect_right);
        killInstance = RuntimeManager.CreateInstance(kill);
        bothDestroyedInstance = RuntimeManager.CreateInstance(bothDestroyed);
        blockedInstance = RuntimeManager.CreateInstance(blocked);
        baseDamageInstance = RuntimeManager.CreateInstance(baseDamage);
        evolveP1 = RuntimeManager.CreateInstance(evolve);
        evolveP2 = RuntimeManager.CreateInstance(evolve); 
    }

    /// <summary>
    /// Play this function when the game starts
    /// </summary>
    public void StartGame()
    {
        musicInstance.setParameterByName("game_phase", 0.0f);
        musicInstance.start();
        gameStartInstance.start();
    }

    public void BaseDamage()
    {
        baseDamageInstance.start();
    }

    public void Evolve(Player player, float duration)
    {
        switch(player)
        {
            case Player.P1:
                evolveP1.setParameterByName("evolve_state", 0.0f);
                evolveP1.start();
                p1EvolveRoutine = StartCoroutine(EvolveTimer(Player.P1, duration));
                break;

            case Player.P2:
                evolveP2.setParameterByName("evolve_state", 0.0f);
                evolveP2.start();
                p2EvolveRoutine = StartCoroutine(EvolveTimer(Player.P2, duration));
                break;
        }
    }

    public void CancelEvolve(Player player)
    {
        switch (player)
        {
            case Player.P1:
                StopCoroutine(p1EvolveRoutine);
                evolveP1.setParameterByName("evolve_state", 0.0f);
                StopAfterTimer(evolveP1, 1.0f);
                break;

            case Player.P2:
                StopCoroutine(p2EvolveRoutine);
                evolveP2.setParameterByName("evolve_state", 0.0f);
                StopAfterTimer(evolveP2, 1.0f);
                break;
        }
    }

    public IEnumerator StopAfterTimer(EventInstance inst, float timer)
    {
        yield return new WaitForSeconds(timer);
        inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private IEnumerator EvolveTimer(Player player, float duration)
    {
        float timer = 0.0f;
        EventInstance inst;
        if (player == Player.P1)
        {
            inst = evolveP1;
        }
        else
        {
            inst = evolveP2;
        }

        while (timer < duration)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            inst.setParameterByName("evolve_state", timer / duration);
        }
        yield return new WaitForSeconds(1.0f);
        inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void NFCDetectLeft()
    {
        nfc_detect_leftInstance.start();
    }

    public void NFCDetectRight()
    {
        nfc_detect_rightInstance.start();
    }

    public void Kill()
    {
        killInstance.start();
    }

    public void BothDestroyed()
    {
        bothDestroyedInstance.start();
    }

    public void Blocked()
    {
        blockedInstance.start();
    }

    public void StartDrawPhase()
    {
        musicInstance.setParameterByName("game_phase", 0.75f);
    }

    public void EndDrawPhase()
    {
        musicInstance.setParameterByName("game_phase", 0.0f);
    }


    
}
