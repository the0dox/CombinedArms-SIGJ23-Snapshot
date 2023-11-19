using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIPostProcessingToggle : MonoBehaviour
{
    private Toggle _myComponent;
    // Start is called before the first frame update
    void Start()
    {
        _myComponent = GetComponent<Toggle>();
        _myComponent.isOn = GameManager.PostProcessing;
    }
}
