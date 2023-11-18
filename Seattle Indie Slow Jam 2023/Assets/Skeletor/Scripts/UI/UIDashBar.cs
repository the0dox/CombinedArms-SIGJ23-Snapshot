using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIDashBar : MonoBehaviour
{
    private Image _dashBar;

    // assign listener on start
    void Start()
    {
        _dashBar = GetComponent<Image>();
        PlayerManager.instance.GetComponent<FirstPersonController>().DashChanged += (object handler, float percentage) => _dashBar.fillAmount = percentage;
    }
}
