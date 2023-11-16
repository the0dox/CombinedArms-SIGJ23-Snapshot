using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// created by skeletor
// attach to an object with an Image component to fade it out over time
public class UIFadeOut : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private bool _fading;
    [SerializeField] private Image _imageComponent;
    private Color _defaultColor;
    private float _alpha;

    void Awake()
    {
        _defaultColor = _imageComponent.color;
    }

    void OnEnable()
    {
        FadeOut();
    }

    void Update()
    {
        if(_fading)
        {
            _alpha -= Time.deltaTime * _fadeSpeed;
            _imageComponent.color = new Color(_defaultColor.r, _defaultColor.b, _defaultColor.g, _alpha);
            if(_alpha < 0)
            {
                Debug.Log("hiding");
                gameObject.SetActive(false);
            }
        }
    }

    public void FadeOut()
    {
        if(!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        else
        {
            _imageComponent.color = _defaultColor;
            _alpha = _defaultColor.a;
        }
    }
}

