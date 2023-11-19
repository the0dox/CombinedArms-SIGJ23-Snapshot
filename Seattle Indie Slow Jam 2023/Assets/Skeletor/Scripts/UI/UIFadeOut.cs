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
    private float _targetAlpha;
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
            if(_alpha > _targetAlpha)
            {
                _alpha -= Time.deltaTime * _fadeSpeed;
                _imageComponent.color = new Color(_defaultColor.r, _defaultColor.b, _defaultColor.g, _alpha);
            }
            else
            {
                _fading = false;
                if(_targetAlpha == 0)
                {
                    Debug.Log("hiding");
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void FadeOut(float targetAlpha = 0)
    {
        _fading = true;
        _targetAlpha = targetAlpha;
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

