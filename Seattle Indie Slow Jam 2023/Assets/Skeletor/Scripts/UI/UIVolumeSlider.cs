using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIVolumeSlider : MonoBehaviour
{
    private Slider _myComponent;
    // Start is called before the first frame update
    void Start()
    {
        _myComponent = GetComponent<Slider>();
        _myComponent.value = GameManager.VolumeLevel;
    }
}
