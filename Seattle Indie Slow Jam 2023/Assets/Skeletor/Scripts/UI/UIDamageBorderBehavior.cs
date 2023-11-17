using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// simple script that flashes red whenever the player takes damage for a brief period
public class UIDamageBorderBehavior : MonoBehaviour
{
    [SerializeField] private UIFadeOut redBorder;

    // assign listener on start
    void Start()
    {
        redBorder.gameObject.SetActive(false);
        PlayerManager.instance.OnPlayerInjured += (object handler, Vector3 direction) => redBorder.FadeOut();
    }

}
