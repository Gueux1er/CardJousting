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

    private EventInstance blockedInstance;
    private EventInstance bothDestroyedInstance;
    private EventInstance countdownInstance;
    private EventInstance evolveInstance;
    private EventInstance gameStartInstance;
    private EventInstance invocationInstance;
    private EventInstance killInstance;
    private EventInstance musicInstance;
    private EventInstance nfc_detect_leftInstance;
    private EventInstance nfc_detect_rightInstance;


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
