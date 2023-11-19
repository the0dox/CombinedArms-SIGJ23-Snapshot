using System.Collections;
using UnityEngine;
using GameFilters;
using System.ComponentModel;

// created by skeletor
// behavior for the dynamic music player can cross fade between different song tracks
// for proper syncage tracks should be the same length and BPM
public class MusicPlayer : MonoBehaviour
{
    // one instance of a music player in any scene
    private static MusicPlayer s_instance;
    // public getter that can also be used as an alternative way to increment the music layer
    public static int CurrentLayer {get=> s_instance._currentLayer; 
    set
        {
            if(value < 0)
            {
                Stop();
            }
            else
            {
                PlayLayer(value);
            }
        }
    } 
    // editable list of all tracks this music player can play
    [SerializeField] private MusicLayer[] _musicLayers;
    // the time in seconds at which songs fade in
    [SerializeField, Range(0, 1)] private float _fadeInSpeed; 
    // the time in seconds at which songs fade out
    [SerializeField, Range(0, 1)] private float _fadeOutSpeed; 
    // reference the to the pair of audio sources attached to this behavior
    private AudioSource[] _audioChannels;
    // reference to index of the active audio source, used swap between sources when cross fading tracks
    private static int s_activeSource;
    // displays the active layer in editor
    [SerializeField] private int _currentLayer;
    // if set to true, the music player will play whatever current layer is set to on awake
    [SerializeField] private bool _playOnAwake;

    // assign instance before first frame
    void Awake()
    {
        if(s_instance == null)
        {
            s_instance = this;
            _audioChannels = GetComponents<AudioSource>();
            s_activeSource = -1;
            if(_playOnAwake)
            {
                PlayLayer(_currentLayer);
            }   
            else
            {
                _currentLayer = -1;
            }
        }
        else
        {
            Debug.LogWarning("Multiple Music Players detected in the scene! For future reference there should only be one", gameObject);
            gameObject.SetActive(false);
        }
    }

    // unassign reference on scene change
    void OnDestroy()
    {
        s_instance = null;
    }

    // plays a music layer from the clips name directly instead of by index 
    public static void PlayLayer(string clipName)
    {
        for(int i = 0; i < s_instance._musicLayers.Length; i++)
        {
            if(s_instance._musicLayers[i].Main.name.Equals(clipName))
            {
                PlayLayer(i);
                return;
            }
        }
        Debug.LogError($"Could not find clip with name {clipName} ensure that the clip is assigned in the music player prefab");
    }

    // finds an music layer that matches key and begins playing it
    // fades out any exisitng tracks 
    public static void PlayLayer(int layer)
    {
        if(!s_instance._musicLayers.WithinRange(layer))
        {
            Debug.LogWarning($"Music Player was asked to play a layer at invalid index {layer}");
            return;
        }
        MusicLayer newLayer = s_instance._musicLayers[layer];
        // if no music is playing check to see if layer has an intro
        if(s_activeSource == -1)
        {
            // if it has an intro play it, else fade layer in as normal
            if(newLayer.Intro != null)
            {
                PlayIntro(newLayer);
                return;    
            }
            else
            {
                s_activeSource = 0;
            }
        }
        AudioSource activeChannel = s_instance._audioChannels[s_activeSource];
        if(activeChannel.clip == newLayer.Main)
        {
            Debug.Log($"Music Player was asked to play a Layer that was already active (Layer {layer})");
            return;
        }
        s_instance._currentLayer = layer;
        AudioSource inactiveChannel = s_instance._audioChannels[1 - s_activeSource];
        float time = activeChannel.time;
        s_instance.StartCoroutine(FadeOutPlayer(activeChannel));
        s_instance.StartCoroutine(FadeInPlayer(inactiveChannel, time, newLayer.Main));
        s_activeSource = s_activeSource == 0 ? 1 : 0;
    }

    // fades out all music layers
    public static void Stop()
    {
        s_activeSource = -1;
        s_instance._currentLayer = -1;
        s_instance.StopAllCoroutines();
        s_instance.StartCoroutine(FadeOutPlayer(s_instance._audioChannels[0]));
        s_instance.StartCoroutine(FadeOutPlayer(s_instance._audioChannels[1]));
        s_instance.StartCoroutine(FadeOutPlayer(s_instance._audioChannels[2]));
    }

    // abruptly ends all music layers
    public static void StopInstant()
    {
        s_activeSource = -1;
        s_instance._currentLayer = -1;
        s_instance.StopAllCoroutines();
        s_instance._audioChannels[0].Stop();
        s_instance._audioChannels[0].clip = null;
        s_instance._audioChannels[1].Stop();
        s_instance._audioChannels[1].clip = null;
        s_instance._audioChannels[2].Stop();
        s_instance._audioChannels[2].clip = null;
    }

    // plays the intro the corresponding layer, then transitions to the main loop. this process can be interupted
    public static void PlayIntro(MusicLayer layer)
    {
        StopInstant();
        s_instance.StartCoroutine(FadeInPlayer(s_instance._audioChannels[2], 0, layer.Intro));
        s_activeSource = 0;
        AudioSource mainChannel = s_instance._audioChannels[s_activeSource];
        mainChannel.clip = layer.Main;
        mainChannel.volume = 1;
        mainChannel.PlayDelayed(layer.Intro.length);
    }

    // plays a specific layer, matching a songLocation for seamless fading
    private static IEnumerator FadeInPlayer(AudioSource player, float time, AudioClip clip)
    {  
        player.clip = clip;
        player.time = time;
        player.volume = 0;
        player.Play();
        while(player.volume < 1)
        {
            player.volume += 0.01f;
            yield return new WaitForSeconds(s_instance._fadeInSpeed /100);
        }
        player.volume = 1;
    }

    // stops a specific layer
    private static IEnumerator FadeOutPlayer(AudioSource player)
    {
        while(player.volume > 0)
        {
            player.volume -= 0.01f;
            yield return new WaitForSeconds(s_instance._fadeOutSpeed/100);
        }
        player.Stop();
        player.clip = null;
    }

    public override string ToString()
    {
        return string.Format($"Music Player Audio Source: {s_activeSource} Current Layer {_currentLayer}");
    }

    /* used for testing purposes
    #if UNITY_EDITOR

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayLayer(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayLayer(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayLayer(2);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            Stop();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayLayer("I'm invalid!");
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
            PlayLayer(0);
        }
        else if(GUILayout.Button("2: Play layer 2"))
        {
            PlayLayer(1);
        }
        else if(GUILayout.Button("3: Play layer 3"))
        {
            PlayLayer(2);
        }
        else if(GUILayout.Button("4: test invalid string invoke"))
        {
            PlayLayer("I'm invalid!");
        }
    }
    #endif
    */
}

// publicily editable music layer
[System.Serializable]
public struct MusicLayer
{
    public AudioClip Main;
    public AudioClip Intro;
}