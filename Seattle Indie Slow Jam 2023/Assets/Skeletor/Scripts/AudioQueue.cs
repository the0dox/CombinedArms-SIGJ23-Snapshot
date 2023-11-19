using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// central audio queue system to play sounds centered on the player 
public class AudioQueue : MonoBehaviour
{
    private static AudioQueue s_instance;
    private Dictionary<string,int> _playedSounds = new Dictionary<string, int>();
    private Queue<AudioSource> _sources = new Queue<AudioSource>();
    [SerializeField] private float _maximumSoundPerType = 3;
    [SerializeField, Range(0,1)] private float _baseVolume; 

    // assign instance before first frame
    void Awake()
    {
        if(s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Debug.LogWarning("More than one audio queue in the scene!, do you have more than one player object?");
            gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if(_sources.Count > 0)
            PlayQueue();
    }

    void PlayQueue()
    {
        _playedSounds.Clear();
        float soundModifier = _baseVolume/_sources.Count;
        while(_sources.Count > 0)
        {
            AudioSource currentSource = _sources.Dequeue();
            if(!_playedSounds.ContainsKey(currentSource.clip.name))
            {
                _playedSounds.Add(currentSource.clip.name, 0);
            }
            int count = _playedSounds[currentSource.clip.name]++;
            if(count < _maximumSoundPerType)
            {
                //Debug.Log("playing " + currentSource.clip.name + " at " + (1 - count/s_instance._maximumSoundPerType) + "volume");
                currentSource.PlayOneShot(currentSource.clip, 1 - count/s_instance._maximumSoundPerType * soundModifier);
            }
        }
    }

    public static void PlaySound(AudioSource sound)
    {
        if(s_instance == null)
        {
            Debug.LogError("No Audio Source Present in Scene!");
        }
        s_instance._sources.Enqueue(sound);
    }
}
