using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// behavior for the dynamic music player can cross fade between different song tracks
// for proper syncage tracks should be the same length and BPM
public class MusicPlayer : MonoBehaviour
{
    // one instance of a music player in any scene
    private static MusicPlayer s_instance;
    // editable list of all tracks this music player can play
    [SerializeField] private MusicLayer[] musicLayers;
    // reference to output of music, used to adjust volume settings
    private AudioSource currentMusic;
    // the time in seconds at which songs fade in
    [SerializeField, Range(0, 1)] private float _fadeInSpeed; 
    // the time in seconds at which songs fade out
    [SerializeField, Range(0, 1)] private float _fadeOutSpeed; 

    // assign instance before first frame
    void Awake()
    {
        s_instance = this;
    }

    // finds an music layer that matches key and begins playing it
    // fades out any exisitng tracks 
    public static void PlayLayer(string key)
    {
        float songLocation = 0;
        if(s_instance.currentMusic != null)
        {
            songLocation = s_instance.currentMusic.time;
        }
        foreach(MusicLayer layer in s_instance.musicLayers)
        {
            if(layer.key.Equals(key))
            {
                s_instance.StartCoroutine(PlayMusic(layer.music, songLocation));
            }
            else
            {
                s_instance.StartCoroutine(StopMusic(layer.music));
            }
        }
    }

    // fades out all music layers
    private static void Stop()
    {
        s_instance.StopAllCoroutines();
        foreach(MusicLayer layer in s_instance.musicLayers)
        {
            s_instance.StartCoroutine(StopMusic(layer.music));
        }
        s_instance.currentMusic = null;
    }

    // plays a specific layer, matching a songLocation for seamless fading
    private static IEnumerator PlayMusic(AudioSource music, float songLocation)
    {
        s_instance.currentMusic = music;
        music.time = songLocation;
        music.volume = 0;
        music.Play();
        while(music.volume < 1)
        {
            music.volume += 0.01f;
            yield return new WaitForSeconds(s_instance._fadeInSpeed /100);
        }
        music.volume = 1;
    }

    // stops a specific layer
    private static IEnumerator StopMusic(AudioSource music)
    {
        while(music.volume > 0)
        {
            music.volume -= 0.01f;
            yield return new WaitForSeconds(s_instance._fadeOutSpeed/100);
        }
        music.volume = 0;
        music.Stop();
    }

    // used for testing purposes
    #if UNITY_EDITOR

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayLayer("layer 1");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayLayer("layer 2");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayLayer("layer 3");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            Stop();
        }
    }

    void OnGUI()
    {
        if(GUILayout.Button("0: Stop"))
        {
            Stop();
        }
        else if(GUILayout.Button("1: Play layer 1"))
        {
            PlayLayer("layer 1");
        }
        else if(GUILayout.Button("2: Play layer 2"))
        {
            PlayLayer("layer 2");
        }
        else if(GUILayout.Button("3: Play layer 3"))
        {
            PlayLayer("layer 3");
        }
    }
    #endif

}

// editor friendly struct for assigning audio sources to string keys
[System.Serializable]
public struct MusicLayer
{
    public string key;
    public AudioSource music;
}
