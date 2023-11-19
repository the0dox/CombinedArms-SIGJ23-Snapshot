using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// created by skeletor
// simple script that flashes red whenever the player takes damage for a brief period
public class UIDamageBorderBehavior : MonoBehaviour
{
    [SerializeField] private UIFadeOut redBorder;
    [SerializeField] private GameObject _lowHealthMessage;
    float previousPercentage = 1;
    float lowHealthMessageThreshold = 0.3f;

    // assign listener on start
    void Start()
    {
        redBorder.gameObject.SetActive(false);
        _lowHealthMessage.gameObject.SetActive(false);
        PlayerManager.instance.OnHealthChanged += OnHealthChanged;
    }

    public void OnHealthChanged(object handler, float newPercentage)
    {
        if(previousPercentage > newPercentage)
        {
            redBorder.FadeOut(1-newPercentage);
        }
        _lowHealthMessage.SetActive(newPercentage < lowHealthMessageThreshold);
        previousPercentage = newPercentage;
    }
}

