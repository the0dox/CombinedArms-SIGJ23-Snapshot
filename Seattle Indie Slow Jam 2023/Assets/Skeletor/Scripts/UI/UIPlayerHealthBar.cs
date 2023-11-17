using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// created by skeletor
// tracks the hp of the player
[RequireComponent(typeof(Slider))]
public class UIPlayerHealthBar : MonoBehaviour
{
    private Slider _healthBar;


    // assign listener on start
    void Start()
    {
        _healthBar = GetComponent<Slider>();
        PlayerManager.instance.OnHealthChanged += (object handler, float percentage) => _healthBar.value = percentage;
    }
}
