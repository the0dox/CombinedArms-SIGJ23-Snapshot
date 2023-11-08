using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// created by skeletor
// processes unity animation events for other behaviors attached to this game object
public class AnimationEventHandler : MonoBehaviour
{
    public Vector3 RootPositionDelta  {get; private set;}
    public Quaternion RootRotationDelta {get; private set;}
    private Animator _animationComponent;

    void Awake()
    {
        _animationComponent = GetComponent<Animator>();
    }

    // called by an animation event directly with value eventMsg, raises that event to any listeners
    public void RaiseAnimationEvent(string eventMsg)
    {
        SendMessageUpwards(eventMsg);
    }

    void OnAnimatorMove()
    {
        RootPositionDelta = _animationComponent.deltaPosition;
        RootRotationDelta = _animationComponent.deltaRotation;
    }
}
