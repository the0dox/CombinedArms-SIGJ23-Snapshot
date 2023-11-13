using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    [SerializeField] private float _startingScale;
    [SerializeField] private float _finalScale;
    [SerializeField] private float _speed;
    private float _scale;
    void OnEnable()
    {
        _scale = _startingScale;
    }

    // Update is called once per frame
    void Update()
    {
        _scale += Time.deltaTime * _speed;
        transform.localScale = Vector3.one * _scale;
        if(_scale > _finalScale)
        {
            gameObject.SetActive(false);
        }
    }
}
