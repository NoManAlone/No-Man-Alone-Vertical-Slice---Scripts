using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioMixerSnapshot lobby, normalView, consoleView;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void OnLevelWasLoaded()
    {
        if (Application.loadedLevelName == "Lobby")
            Lobby();

        if (Application.loadedLevelName == "Level 1")
            NormalView();
    }

    //Mixer Snapshots
    public void Lobby()
    {
        lobby.TransitionTo(0.1f);
    }

    public void ConsoleView()
    {
        consoleView.TransitionTo(0.1f);
    }

    public void NormalView()
    {
        normalView.TransitionTo(0.1f);
    }
}
