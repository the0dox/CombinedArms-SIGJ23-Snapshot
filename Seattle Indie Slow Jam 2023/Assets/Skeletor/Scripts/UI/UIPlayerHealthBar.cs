using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// created by skeletor
// tracks the hp of the player
[RequireComponent(typeof(Image))]
public class UIPlayerHealthBar : MonoBehaviour
{
    private Image _healthBar;


    // assign listener on start
    void Start()
    {
        _healthBar = GetComponent<Image>();
        PlayerManager.instance.OnHealthChanged += (object handler, float percentage) => _healthBar.fillAmount = percentage;
    }
}
