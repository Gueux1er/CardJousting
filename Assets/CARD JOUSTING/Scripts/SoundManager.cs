using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using EventInstance = FMOD.Studio.EventInstance;
using RuntimeManager = FMODUnity.RuntimeManager;
using LibLabSystem;
using LibLabGames.NewGame;

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
    [EventRef]
    public string summon;

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
    private EventInstance summonInstance;

    private EventInstance evolveP1;
    private EventInstance evolveP2;
    private IEnumerator p1EvolveRoutine;
    private IEnumerator p2EvolveRoutine;

    public enum Player { P1, P2 }

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this);
            return;
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
        summonInstance = RuntimeManager.CreateInstance(summon);
    }

    /// <summary>
    /// Play this function when the game starts
    /// </summary>
    public void StartGame()
    {
        musicInstance.setParameterByName("game_phase", 0.75f);
        musicInstance.start();
        gameStartInstance.start();
    }

    public void BaseDamage()
    {
        baseDamageInstance.start();
    }

    public void Summon()
    {
        summonInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        summonInstance.start();
    }

    public void Evolve(Player player, float duration)
    {
        switch (player)
        {
        case Player.P1:
            evolveP1.setParameterByName("evolve_state", 0.0f);
            evolveP1.start();

            if (p1EvolveRoutine != null)
                StopCoroutine(p1EvolveRoutine);
            
            p1EvolveRoutine = EvolveTimer(Player.P1, duration);
            StartCoroutine(p1EvolveRoutine);
            break;

        case Player.P2:
            evolveP2.setParameterByName("evolve_state", 0.0f);
            evolveP2.start();

            if (p2EvolveRoutine != null)
                StopCoroutine(p2EvolveRoutine);

            p2EvolveRoutine = EvolveTimer(Player.P2, duration);
            StartCoroutine(p2EvolveRoutine);
            break;
        }
    }

    public void ResetSound()
    {
        if (p1EvolveRoutine != null)
            StopCoroutine(p1EvolveRoutine);

        if (p2EvolveRoutine != null)
            StopCoroutine(p2EvolveRoutine);

        evolveP1.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        evolveP2.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void CancelEvolve(Player player)
    {
        switch (player)
        {
        case Player.P1:
            StopCoroutine(p1EvolveRoutine);
            evolveP1.setParameterByName("evolve_state", 0.0f);
            StartCoroutine(StopAfterTimer(evolveP1, 1.0f));
                break;

        case Player.P2:
            StopCoroutine(p2EvolveRoutine);
            evolveP2.setParameterByName("evolve_state", 0.0f);
            StartCoroutine(StopAfterTimer(evolveP2, 1.0f));
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
            while (GameManager.instance.isDrawPhase)
                yield return null;

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            inst.setParameterByName("evolve_state", timer / duration);
        }

        yield return new WaitForSeconds(1f);

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
        killInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        killInstance.start();
    }

    public void BothDestroyed()
    {
        bothDestroyedInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        bothDestroyedInstance.start();
    }

    public void Blocked()
    {
        blockedInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        blockedInstance.start();
    }

    public void StartDrawPhase()
    {
        musicInstance.setParameterByName("game_phase", 0.75f);
        evolveP1.setPaused(true);
        evolveP2.setPaused(true);
    }

    public void EndDrawPhase()
    {
        musicInstance.setParameterByName("game_phase", 0.0f);
        evolveP1.setPaused(false);
        evolveP2.setPaused(false);
    }

}